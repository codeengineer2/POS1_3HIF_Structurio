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

        public void GoToSignInPage()
        {
            loginFrame.Navigate(new SignInPage(this));
        }

        public void GoToSignUpPage()
        {
            loginFrame.Navigate (new SignUpPage());
        }

        // todo -> make spinning animation

        public async void ShowSpinningAnimation()
        {
            // here -> load spinning animation
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