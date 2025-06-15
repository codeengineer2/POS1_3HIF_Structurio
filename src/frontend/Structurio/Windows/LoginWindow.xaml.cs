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
using Structurio.Interfaces;
using Structurio.Services;

namespace Structurio.Windows
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private IApiService api = new ApiService();

        public LoginWindow()
        {
            InitializeComponent();
            loginFrame.Navigate(new SignInPage(this, api));
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
            loginFrame.Navigate(new SignInPage(this, api));
        }

        public void GoToSignUpPage()
        {
            loginFrame.Navigate(new SignUpPage(this, api));
        }
    }
}