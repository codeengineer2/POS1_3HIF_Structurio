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
using Structurio.Classes;
using Structurio.Services;
using Structurio.Windows;
using System.Runtime.InteropServices;

namespace Structurio.Controls
{
    public partial class ProjectIssueControl : UserControl
    {
        private bool isMouseDown = false;
        private GhostWindow ghostWindow = null;

        public Issue Issue => this.DataContext as Issue;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT { public int X; public int Y; }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;

        public Point GetCursorPositionScaled()
        {
            GetCursorPos(out POINT point);
            IntPtr hdc = GetDC(IntPtr.Zero);
            int dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
            int dpiY = GetDeviceCaps(hdc, LOGPIXELSY);
            ReleaseDC(IntPtr.Zero, hdc);

            double scaleX = dpiX / 96.0;
            double scaleY = dpiY / 96.0;

            return new Point(point.X / scaleX, point.Y / scaleY);
        }

        public ProjectIssueControl()
        {
            InitializeComponent();

            rootBorder.MouseMove += rootBorder_MouseMove;
            rootBorder.MouseLeftButtonDown += rootBorder_MouseLeftButtonDown;
            rootBorder.MouseLeftButtonUp += rootBorder_MouseLeftButtonUp;
        }

        private void rootBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
            rootBorder.CaptureMouse();
        }

        private void rootBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
            rootBorder.ReleaseMouseCapture();
        }

        private void rootBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && isMouseDown && ghostWindow == null)
            {
                var clone = new ProjectIssueControl { DataContext = this.DataContext };
                clone.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                clone.Arrange(new Rect(clone.DesiredSize));
                clone.UpdateLayout();

                var bounds = VisualTreeHelper.GetDescendantBounds(clone);
                var renderTargetBitmap = new RenderTargetBitmap((int)Math.Ceiling(bounds.Width), (int)Math.Ceiling(bounds.Height), 96, 96, PixelFormats.Pbgra32);

                var drawingVisual = new DrawingVisual();
                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(new VisualBrush(clone), null, new Rect(new Point(), bounds.Size));
                }
                renderTargetBitmap.Render(drawingVisual);

                var image = new Image
                {
                    Source = renderTargetBitmap,
                    Width = bounds.Width,
                    Height = bounds.Height,
                    IsHitTestVisible = false
                };

                ghostWindow = new GhostWindow(image);

                Point scaledCursor = GetCursorPositionScaled();
                ghostWindow.Left = scaledCursor.X - 20;
                ghostWindow.Top = scaledCursor.Y + 20;

                ghostWindow.Show();

                CompositionTarget.Rendering += FollowCursor;

                var issue = DataContext as Issue;
                if (issue != null)
                {
                    Mouse.OverrideCursor = Cursors.Hand;
                    DragDrop.DoDragDrop(this, new DataObject("Issue", issue), DragDropEffects.Move);
                    Mouse.OverrideCursor = null;

                    CompositionTarget.Rendering -= FollowCursor;

                    ghostWindow?.Close();
                    ghostWindow = null;
                }
            }
        }

        private void FollowCursor(object sender, EventArgs e)
        {
            if (ghostWindow != null)
            {
                Point scaledCursor = GetCursorPositionScaled();
                ghostWindow.Left = scaledCursor.X - 20;
                ghostWindow.Top = scaledCursor.Y + 20;
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