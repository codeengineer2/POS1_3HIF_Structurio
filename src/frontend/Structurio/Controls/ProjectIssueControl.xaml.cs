using Structurio.Classes;
using Structurio.Services;
using Structurio.Windows;
using System;
using System.Collections.Generic;
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

namespace Structurio.Controls
{
    /// <summary>
    /// Interaction logic for ProjectIssueControl.xaml
    /// </summary>
    public partial class ProjectIssueControl : UserControl
    {
        public Issue Issue => this.DataContext as Issue;

        public ProjectIssueControl()
        {
            InitializeComponent();
            rootBorder.MouseMove += rootBorder_MouseMove;
        }

        private void rootBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var issue = DataContext as Issue;
                if (issue != null)
                {
                    DragDrop.DoDragDrop(this, new DataObject("Issue", issue), DragDropEffects.Move);
                }
            }
        }

        private void IssueControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Issue"))
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = false;
            }
        }

        private void IssueControl_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("Issue"))
            {
                return;
            }

            var sourceIssue = e.Data.GetData("Issue") as Issue;
            var targetIssue = DataContext as Issue;

            if (sourceIssue == null || targetIssue == null || sourceIssue == targetIssue)
            {
                return;
            }

            if (sourceIssue.ColumnId == targetIssue.ColumnId)
            {
                var itemsControl = FindItemsControl();
                if (itemsControl?.DataContext is ColumnWrapper column)
                {
                    int oldIndex = column.Items.IndexOf(sourceIssue);
                    int newIndex = column.Items.IndexOf(targetIssue);
                    if (oldIndex >= 0 && newIndex >= 0 && oldIndex != newIndex)
                    {
                        column.Items.Move(oldIndex, newIndex);
                        e.Handled = true; 
                    }
                }
            }
        }

        private ItemsControl FindItemsControl()
        {
            DependencyObject parent = this;
            while (parent != null)
            {
                if (parent is ItemsControl itemsControl)
                {
                    return itemsControl;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        private async void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new UpdateIssueWindow(this.Issue);
            window.Owner = Window.GetWindow(this);

            if (window.ShowDialog() == true)
            {
                if (window.IsDeleted)
                {
                    var api = new ApiService();
                    var success = await api.DeleteIssueAsync(this.Issue.Id);

                    if (success)
                    {
                        var column = FindParentColumn();

                        if (column != null)
                        {
                            column.Original.Issues.Remove(this.Issue);
                            column.Items.Remove(this.Issue);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Fehler beim Löschen des Issues!");
                    }
                }
                else
                {
                    var api = new ApiService();
                    var updateRequest = new UpdateIssueRequest
                    {
                        Id = this.Issue.Id,
                        Description = window.UpdatedDescription
                    };

                    var success = await api.UpdateIssueAsync(updateRequest);

                    if (success)
                    {
                        this.Issue.Description = window.UpdatedDescription;
                    }
                    else
                    {
                        MessageBox.Show("Fehler beim Aktualisieren des Issues!");
                    }
                }
            }
        }
        
        private ColumnWrapper FindParentColumn()
        {
            DependencyObject parent = this;
            while (parent != null)
            {
                if (parent is FrameworkElement frameworkElement && frameworkElement.DataContext is ColumnWrapper column)
                {
                    return column;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
    }
}