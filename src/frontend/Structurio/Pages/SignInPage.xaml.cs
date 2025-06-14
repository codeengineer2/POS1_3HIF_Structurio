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
using Structurio.Windows;
using Structurio.Classes;
using Structurio.Services;
using Structurio.Interfaces;
using System.Text.RegularExpressions;

namespace Structurio.Pages
{
    /// <summary>
    /// Interaktionslogik für SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        private LoginWindow loginWindow;
        private bool isPasswordVisible = false;
        private IApiService api;
        private Regex emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
        private Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,64}$", RegexOptions.Compiled);

        public SignInPage(LoginWindow loginWindow, IApiService api)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
            this.api = api;

            // blockiert copy/paste
            CommandManager.AddPreviewExecutedHandler(passwordBox, BlockCopyPasteCommand);
            CommandManager.AddPreviewExecutedHandler(passwordTextBox, BlockCopyPasteCommand);
        }

        private void BlockCopyPasteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void forgotPassword_Click(object sender, RoutedEventArgs e)
        {
            loginWindow.GoToPasswordResetPage();
        }

        private void GoToSignUp_Click(object sender, RoutedEventArgs e)
        {
            loginWindow.GoToSignUpPage();
        }
        private void emailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            emailBox.Background = Brushes.White;
            emailInfo.Text = "* erforderlich";
            emailInfo.Foreground = Brushes.Gray;
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordBox.Background = Brushes.White;
            passwordInfo.Text = "* erforderlich";
            passwordInfo.Foreground = Brushes.Gray;
        }

        private void passwordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            passwordTextBox.Background = Brushes.White;
            passwordInfo.Text = "* erforderlich";
            passwordInfo.Foreground = Brushes.Gray;
        }

        private void togglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                passwordTextBox.Text = passwordBox.Password;
                passwordBox.Visibility = Visibility.Collapsed;
                passwordTextBox.Visibility = Visibility.Visible;

                eyeImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/login_eye_closed.png"));
            }
            else
            {
                passwordBox.Password = passwordTextBox.Text;
                passwordBox.Visibility = Visibility.Visible;
                passwordTextBox.Visibility = Visibility.Collapsed;

                eyeImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/login_eye_open.png"));
            }
        }

        private async void login_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            int maxEmailLength = 100;
            int minPasswordLength = 8;
            int maxPasswordLength = 64;

            string email = emailBox.Text.Trim();
            string password = isPasswordVisible ? passwordTextBox.Text : passwordBox.Password;

            emailBox.Background = Brushes.White;
            passwordBox.Background = Brushes.White;
            passwordTextBox.Background = Brushes.White;
            emailInfo.Text = "* erforderlich";
            passwordInfo.Text = "* erforderlich";
            emailInfo.Foreground = Brushes.Gray;
            passwordInfo.Foreground = Brushes.Gray;

            if (string.IsNullOrWhiteSpace(email))
            {
                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Bitte ausfüllen!";
                emailInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (email.Length > maxEmailLength)
            {
                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = $"E-Mail darf maximal {maxEmailLength} Zeichen haben.";
                emailInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (!emailRegex.IsMatch(email))
            {
                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Ungültiges Format (z. B. name@domain.com)";
                emailInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                if (isPasswordVisible)
                {
                    passwordTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                }
                else
                {
                    passwordBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                }

                passwordInfo.Text = "Bitte ausfüllen!";
                passwordInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (password.Length > maxPasswordLength)
            {
                if (isPasswordVisible)
                {
                    passwordTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                }
                else
                {
                    passwordBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                }

                passwordInfo.Text = $"Maximal {maxPasswordLength} Zeichen erlaubt.";
                passwordInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (!passwordRegex.IsMatch(password))
            {
                if (isPasswordVisible)
                {
                    passwordTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                }
                else
                {
                    passwordBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                }

                passwordInfo.Text = "Mind. 1 Groß-, 1 Kleinbuchstabe, Zahl, Sonderzeichen";
                passwordInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            if (!valid)
            {
                return;
            }

            loginWindow.SpinningAnimation();
            var result = await api.LoginAsync(email, password);
            loginWindow.ResetSpinningAnimation();

            if (result != null && result.Success)
            {
                loginWindow.GoToMainWindow(result.User, result.Projects);
            }
            else
            {
                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                if (isPasswordVisible)
                {
                    passwordTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                }
                else
                {
                    passwordBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                }

                emailInfo.Text = "Falsche Zugangsdaten!";
                passwordInfo.Text = "Falsche Zugangsdaten!";
                emailInfo.Foreground = Brushes.DarkRed;
                passwordInfo.Foreground = Brushes.DarkRed;
            }
        }
    }
}