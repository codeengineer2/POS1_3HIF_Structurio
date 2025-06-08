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

namespace Structurio.Pages
{
    /// <summary>
    /// Interaktionslogik für SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private LoginWindow loginWindow;
        private bool isPasswordVisible = false;

        public SignUpPage(LoginWindow loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;

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

        private void GoToSignInPage_Click(object sender, RoutedEventArgs e)
        {
            loginWindow.GoToSignInPage();
        }

        private void firstNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            firstNameBox.Background = Brushes.White;
            firstNameInfo.Text = "* erforderlich";
            firstNameInfo.Foreground = Brushes.Gray;
        }

        private void lastNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            lastNameBox.Background = Brushes.White;
            lastNameInfo.Text = "* erforderlich";
            lastNameInfo.Foreground = Brushes.Gray;
        }

        private void birthDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            birthDateInfo.Text = "* erforderlich";
            birthDateInfo.Foreground = Brushes.Gray;
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

        private void togglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                passwordTextBox.Text = passwordBox.Password;
                passwordTextBox.Visibility = Visibility.Visible;
                passwordBox.Visibility = Visibility.Collapsed;
                eyeImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/login_eye_closed.png"));
                passwordTextBox.Focus();
                passwordTextBox.CaretIndex = passwordTextBox.Text.Length;
            }
            else
            {
                passwordBox.Password = passwordTextBox.Text;
                passwordTextBox.Visibility = Visibility.Collapsed;
                passwordBox.Visibility = Visibility.Visible;
                eyeImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/login_eye_open.png"));
                passwordBox.Focus();
                passwordBox.SelectAll();
            }
        }

        private async void register_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            if (string.IsNullOrWhiteSpace(firstNameBox.Text))
            {
                firstNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                firstNameInfo.Text = "Bitte ausfüllen!";
                firstNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(lastNameBox.Text))
            {
                lastNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                lastNameInfo.Text = "Bitte ausfüllen!";
                lastNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            if (birthDatePicker.SelectedDate == null)
            {
                birthDateInfo.Text = "Bitte auswählen!";
                birthDateInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(emailBox.Text))
            {
                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Bitte ausfüllen!";
                emailInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            string password = isPasswordVisible ? passwordTextBox.Text : passwordBox.Password;

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

            if (!valid)
            {
                return;
            }

            var request = new RegisterRequest
            {
                Firstname = firstNameBox.Text.Trim(),
                Lastname = lastNameBox.Text.Trim(),
                Email = emailBox.Text.Trim(),
                Password = password,
                Birthdate = birthDatePicker.SelectedDate?.ToString("yyyy-MM-dd") ?? ""
            };

            loginWindow.ShowSpinningAnimation();
            bool success = await ApiService.RegisterAsync(request);
            loginWindow.ResetSpinningAnimation();

            if (success)
            {
                new MainWindow().Show();
                loginWindow.Close();
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }
    }
}