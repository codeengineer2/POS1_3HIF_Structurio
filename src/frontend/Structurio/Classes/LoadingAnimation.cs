using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;

namespace Structurio.Classes
{
    public static class LoadingAnimation
    {
        public static void Start(Canvas canvas, Grid overlay)
        {
            if (canvas == null || overlay == null)
            {
                return;
            }

            canvas.Children.Clear();
            overlay.Visibility = Visibility.Visible;

            double center = canvas.Width / 2;
            double radius = 30;
            int ballCounter = 10;

            for (int i = 0; i < ballCounter; i++)
            {
                double angle = i * 360.0 / ballCounter;
                double rad = angle * Math.PI / 180;
                double x = center + radius * Math.Cos(rad) - 5;
                double y = center + radius * Math.Sin(rad) - 5;

                var ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#1451b8"),
                    Opacity = 0.9
                };

                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                var animation = new DoubleAnimation
                {
                    From = 0.3,
                    To = 1.0,
                    Duration = TimeSpan.FromMilliseconds(900),
                    BeginTime = TimeSpan.FromMilliseconds(i * 100),
                    RepeatBehavior = RepeatBehavior.Forever,
                    AutoReverse = true
                };

                ellipse.BeginAnimation(UIElement.OpacityProperty, animation);
                canvas.Children.Add(ellipse);
            }
        }

        public static void Stop(Canvas canvas, Grid overlay)
        {
            if (canvas == null || overlay == null)
            {
                return;
            }

            canvas.Children.Clear();
            overlay.Visibility = Visibility.Collapsed;
        }

        public static async Task RunAsync(Canvas canvas, Grid overlay, Func<Task> action)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                Start(canvas, overlay);
                await action();
                Mouse.OverrideCursor = null;
            }
            finally
            {
                Stop(canvas, overlay);
                Mouse.OverrideCursor = null;
            }
        }
    }
}