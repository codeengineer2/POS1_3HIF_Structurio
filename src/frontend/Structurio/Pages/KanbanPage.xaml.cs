using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Structurio.Classes;
using Structurio.Controls;
using Structurio.Windows;

namespace Structurio.Pages
{
    /// <summary>
    /// Interaction logic for KanbanPage.xaml
    /// </summary>
    public partial class KanbanPage : Page
    {
        private Project project;
        public ObservableCollection<ColumnWrapper> Columns { get; set; } = new();
        private int issueCounter = 1;
        private Issue issueBeingMoved;

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

        private void addItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ColumnWrapper column)
            {
                var window = new AddIssueWindow { Owner = Window.GetWindow(this) };
                if (window.ShowDialog() == true)
                {
                    var issue = new Issue
                    {
                        Id = issueCounter++,
                        Description = window.IssueDescription,
                        ColumnId = column.Original.Id,
                        Name = project.Name
                    };

                    column.Original.Issues.Add(issue);
                    column.Items.Add(issue);
                }
            }
        }

        private void addColumn_Click(object sender, RoutedEventArgs e)
        {
            int number = Columns.Count + 1;
            var column = new Column
            {
                Name = $"Spalte {number}",
                Issues = new List<Issue>()
            };

            project.Board.Columns.Add(column);
            Columns.Add(new ColumnWrapper(column));
        }

        private void titleBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is ColumnWrapper column)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    int index = Columns.IndexOf(column) + 1;
                    column.Name = $"Spalte {index}";
                    textBox.Text = column.Name;
                    Keyboard.ClearFocus();
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