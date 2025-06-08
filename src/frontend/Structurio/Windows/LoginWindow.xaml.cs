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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Structurio.Pages;
using Structurio.Classes;

namespace Structurio.Windows
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            loginFrame.Navigate(new SignInPage(this));
        }

        public void GoToMainWindow(User user, List<Project> projects)
        {
            var mainWindow = new MainWindow(user, projects);
            mainWindow.Show();
            this.Close();
        }

        public void GoToPasswordResetPage()
        {
            loginFrame.Navigate(new PasswordResetPage(this));
        }

        public void GoToSignInPage()
        {
            loginFrame.Navigate(new SignInPage(this));
        }

        public void GoToSignUpPage()
        {
            loginFrame.Navigate (new SignUpPage(this));
        }

        public void SpinningAnimation() 
        {
            loadingGrid.Visibility = Visibility.Visible;
            loadingAnimationCanvas.Children.Clear();

            double center = 50;
            double radius = 30;
            int ballCounter = 10;

            for (int i = 0; i < ballCounter; i++)
            {
                double angle = i * 360.0 / ballCounter;
                double rad = angle * Math.PI / 180;
                double x = center + radius * Math.Cos(rad) - 5;
                double y = center + radius * Math.Sin(rad) - 5;

                Ellipse ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1451b8")),
                    Opacity = 0.9
                };

                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 0.3,
                    To = 1.0,
                    Duration = TimeSpan.FromMilliseconds(900),
                    BeginTime = TimeSpan.FromMilliseconds(i * 100),
                    RepeatBehavior = RepeatBehavior.Forever,
                    AutoReverse = true
                };

                ellipse.BeginAnimation(UIElement.OpacityProperty, animation);
                loadingAnimationCanvas.Children.Add(ellipse);
            }
        }

        public async void ShowSpinningAnimation()
        {
            SpinningAnimation();
            await Task.Delay(5000);
            ResetSpinningAnimation();

            new MainWindow().Show();
            this.Close();
        }

        public void ResetSpinningAnimation()
        {
            loadingAnimationCanvas.Children.Clear();
            loadingGrid.Visibility = Visibility.Collapsed;
        }
    }
}