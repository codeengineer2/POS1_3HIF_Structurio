using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Structurio.Classes;
using Structurio.Controls;
using Structurio.Interfaces;
using Structurio.Services;
using Structurio.Windows;
using System.Runtime.InteropServices;
namespace Structurio.Pages

{
    /// <summary>
    /// Interaction logic for KanbanPage.xaml
    /// </summary>
    public partial class KanbanPage : Page
    {
        private Project project;
        private IApiService apiService;
        private int issueCounter = 1;
        private Issue issueBeingMoved;
        private DispatcherTimer autoScrollTimer;
        private double ScrollThreshold = 60;
        private double ScrollSpeed = 20;

        public ObservableCollection<ColumnWrapper> Columns { get; set; } = new();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT { public int X; public int Y; }

        public KanbanPage(Project project)
        {
            InitializeComponent();
            this.project = project;

            foreach (var column in project.Board.Columns)
            {
                Columns.Add(new ColumnWrapper(column));
            }

            kanbanItemsControl.ItemsSource = Columns;
        }

        public void StartAutoScroll()
        {
            if (autoScrollTimer == null)
            {
                autoScrollTimer = new DispatcherTimer();
                autoScrollTimer.Interval = TimeSpan.FromMilliseconds(40);
                autoScrollTimer.Tick += AutoScrollTimer_Tick;
            }
            autoScrollTimer.Start();
        }

        public void StopAutoScroll()
        {
            autoScrollTimer?.Stop();
        }

        private void AutoScrollTimer_Tick(object sender, EventArgs e)
        {
            GetCursorPos(out POINT p);
            Point cursorScreen = new Point(p.X, p.Y);
            Point relativeToScroll = scrollViewer.PointFromScreen(cursorScreen);

            if (relativeToScroll.X < ScrollThreshold)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - ScrollSpeed);
            }
            else if (relativeToScroll.X > scrollViewer.ActualWidth - ScrollThreshold)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + ScrollSpeed);
            }
        }

        private async void addItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ColumnWrapper column)
            {
                var window = new AddIssueWindow { Owner = Window.GetWindow(this) };
                window.DataContext = column.Original;

                if (window.ShowDialog() == true)
                {
                    var api = new ApiService();

                    var request = new AddIssueRequest
                    {
                        ColumnId = column.Original.Id,
                        Description = window.IssueDescription
                    };

                    var newIssue = await api.AddIssueAsync(request);

                    if (newIssue != null)
                    {
                        newIssue.Name = project.Name;

                        column.Original.Issues.Add(newIssue);
                        column.Items.Add(newIssue);
                    }
                    else
                    {
                        MessageBox.Show("Fehler beim Erstellen des Issues!");
                    }
                }
            }
        }

        private async void addColumn_Click(object sender, RoutedEventArgs e)
        {
            var api = new ApiService();

            int number = Columns.Count + 1;

            var request = new AddColumnRequest
            {
                BoardId = project.Board.Id,
                Name = $"Spalte {number}"
            };

            var newColumn = await api.AddColumnAsync(request);

            if (newColumn != null)
            {
                newColumn.BoardId = project.Board.Id;
                project.Board.Columns.Add(newColumn);
                Columns.Add(new ColumnWrapper(newColumn));
            }
            else
            {
                MessageBox.Show("Fehler beim Erstellen der Spalte!");
            }
        }

        private async void titleBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is ColumnWrapper column)
            {
                var newName = textBox.Text.Trim();
                var oldName = column.Original.Name;

                if (string.IsNullOrWhiteSpace(newName))
                {
                    int index = Columns.IndexOf(column) + 1;
                    column.Name = $"Spalte {index}";
                    textBox.Text = column.Name;
                    Keyboard.ClearFocus();
                    return;
                }

                column.Name = newName;

                var api = new ApiService();

                var updateRequest = new UpdateColumnRequest
                {
                    Id = column.Original.Id,
                    Name = newName
                };

                var success = await api.UpdateColumnAsync(updateRequest);

                if (!success)
                {
                    MessageBox.Show("Spaltenname konnte nicht gespeichert werden!");
                }
            }
        }

        private void ItemsControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Issue)))
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
        }

        private void ItemsControl_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Issue)) && sender is ItemsControl itemsControl)
            {
                var targetColumn = itemsControl.DataContext as ColumnWrapper;
                var issue = e.Data.GetData(typeof(Issue)) as Issue;

                if (targetColumn != null && issue != null)
                {
                    issueBeingMoved = issue;
                    var currentColumn = Columns.FirstOrDefault(ContainsIssue);
                    issueBeingMoved = null;

                    if (currentColumn != null && currentColumn != targetColumn)
                    {
                        currentColumn.Items.Remove(issue);
                        currentColumn.Original.Issues.Remove(issue);

                        issue.ColumnId = targetColumn.Original.Id;
                        targetColumn.Items.Add(issue);
                        targetColumn.Original.Issues.Add(issue);
                    }
                }
            }
        }

        private void Column_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Issue"))
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
        }

        private void Column_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Issue") && sender is Border border)
            {
                var issue = e.Data.GetData("Issue") as Issue;
                var targetColumn = border.DataContext as ColumnWrapper;

                if (issue != null && targetColumn != null)
                {
                    issueBeingMoved = issue;
                    var currentColumn = Columns.FirstOrDefault(ContainsIssue);
                    issueBeingMoved = null;

                    if (currentColumn != null && currentColumn != targetColumn)
                    {
                        currentColumn.Items.Remove(issue);
                        currentColumn.Original.Issues.Remove(issue);

                        issue.ColumnId = targetColumn.Original.Id;

                        targetColumn.Items.Add(issue);
                        targetColumn.Original.Issues.Add(issue);
                    }
                }
            }
        }

        private bool ContainsIssue(ColumnWrapper column)
        {
            return column.Items.Contains(this.issueBeingMoved);
        }
    }
}