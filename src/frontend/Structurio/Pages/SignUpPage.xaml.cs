using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Newtonsoft.Json;
using System.Net.Http;

namespace Structurio.Pages
{
    /// <summary>
    /// Interaktionslogik für SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private LoginWindow loginWindow;
        private bool isPasswordVisible = false;
        private IApiService api;
        private Regex nameRegex = new Regex(@"^[\p{L}\p{M} \-']{1,50}$", RegexOptions.Compiled);
        private Regex emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
        private Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,64}$", RegexOptions.Compiled);

        public SignUpPage(LoginWindow loginWindow, IApiService api)
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
            birthDatePicker.Background = Brushes.White;
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

        private async Task<bool> CheckEmailAsync(string email)
        {
            var mail = new { email };
            var json = JsonConvert.SerializeObject(mail);

            using var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/auth/check-email", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async void register_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            int recommendedNameLength = 10;
            DateTime minBirthdate = new DateTime(1900, 1, 1);
            DateTime maxBirthdate = DateTime.Today.AddYears(-13);
            int maxEmailLength = 100;
            int minPasswordLength = 8;
            int maxPasswordLength = 64;

            string firstName = firstNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                firstNameInfo.Text = "Bitte ausfüllen!";
                firstNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (firstName.Length > 50)
            {
                firstNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                firstNameInfo.Text = "Vorname darf maximal 50 Zeichen haben.";
                firstNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (!nameRegex.IsMatch(firstName))
            {
                firstNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                firstNameInfo.Text = "Nur Buchstaben, Leerzeichen, - und ' erlaubt.";
                firstNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (firstName.Length > recommendedNameLength)
            {
                firstNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 245, 200));
                firstNameInfo.Text = $"Vorname ist ungewöhnlich lang (max. {recommendedNameLength} empfohlen)";
                firstNameInfo.Foreground = Brushes.DarkOrange;
            }

            string lastName = lastNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(lastName))
            {
                lastNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                lastNameInfo.Text = "Bitte ausfüllen!";
                lastNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (lastName.Length > 50)
            {
                lastNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                lastNameInfo.Text = "Nachname darf maximal 50 Zeichen haben.";
                lastNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (!nameRegex.IsMatch(lastName))
            {
                lastNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                lastNameInfo.Text = "Nur Buchstaben, Leerzeichen, - und ' erlaubt.";
                lastNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (lastName.Length > recommendedNameLength)
            {
                lastNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 245, 200));
                lastNameInfo.Text = $"Nachname ist ungewöhnlich lang (max. {recommendedNameLength} empfohlen)";
                lastNameInfo.Foreground = Brushes.DarkOrange;
            }

            DateTime? birthDate = birthDatePicker.SelectedDate;
            if (birthDate == null)
            {
                birthDatePicker.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                birthDateInfo.Text = "Bitte auswählen!";
                birthDateInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (birthDate < minBirthdate)
            {
                birthDatePicker.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                birthDateInfo.Text = "Bitte wähle ein realistisches Geburtsdatum ab dem Jahr 1900.";
                birthDateInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (birthDate > maxBirthdate)
            {
                birthDatePicker.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                birthDateInfo.Text = "Du musst mindestens 13 Jahre alt sein.";
                birthDateInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            string email = emailBox.Text.Trim();
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
                emailInfo.Text = "E-Mail ist zu lang (maximal 150 Zeichen erlaubt).";
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

            string password = isPasswordVisible ? passwordTextBox.Text.Trim() : passwordBox.Password.Trim();
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
            else if (password.Length < minPasswordLength || password.Length > maxPasswordLength)
            {
                if (isPasswordVisible)
                {
                    passwordTextBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                }
                else
                {
                    passwordBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                }

                passwordInfo.Text = $"Länge ungültig (min. {minPasswordLength}, max. {maxPasswordLength})";
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

                passwordInfo.Text = "Mind. 1 Groß-/Kleinbuchstabe, Zahl & Sonderzeichen";
                passwordInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            if (!valid)
            {
                return;
            }

            await LoadingAnimation.RunAsync(loginWindow.loadingAnimationCanvas, loginWindow.loadingGrid, async () =>
            {
                bool alreadyExists = await CheckEmailAsync(email);
                if (alreadyExists)
                {
                    emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                    emailInfo.Text = "Diese E-Mail ist bereits vergeben!";
                    emailInfo.Foreground = Brushes.DarkRed;
                    return;
                }

                var request = new RegisterRequest
                {
                    Firstname = firstName,
                    Lastname = lastName,
                    Email = email,
                    Password = password,
                    Birthdate = birthDate?.ToString("yyyy-MM-dd") ?? ""
                };

                bool success = await api.RegisterAsync(request);
                if (success)
                {
                    var loginResult = await api.LoginAsync(email, password);
                    if (loginResult?.Success == true)
                    {
                        loginWindow.GoToMainWindow(loginResult.User, loginResult.Projects);
                    }
                    else
                    {
                        MessageBox.Show("Fehler beim Login.");
                    }
                }
                else
                {
                    MessageBox.Show("Registrierung fehlgeschlagen. Bitte versuche es erneut.");
                }
            });
        }
    }
}