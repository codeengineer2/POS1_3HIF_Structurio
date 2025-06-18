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
using Serilog;

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

            Log.Information("SignUpPage wurde geladen.");

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
                Log.Debug("Copy oder Paste oder Cut wurde in der Passwortbox verhindert.");
                e.Handled = true;
            }
        }

        private void GoToSignInPage_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Gehe zur SignInPage.");
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
            Log.Information($"Passwortanzeige wurde umgeschalten zu {isPasswordVisible}.");

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
            Log.Information($"Schaue nach ob es die EMail={email} gibt.");

            var mail = new { email };
            var json = JsonConvert.SerializeObject(mail);

            using var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/auth/check-email", content);
                Log.Information($"CheckEmailAsync abgeschlossen, StatusCode={response.StatusCode}.");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler bei CheckEmailAsync für die EMail={email}.");
                return false;
            }
        }

        private async void register_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Register Button geklickt und somit die Registrierung gestartet.");

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
                Log.Warning($"Ungültiger Vorname={firstName}.");

                firstNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                firstNameInfo.Text = "Bitte ausfüllen!";
                firstNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (firstName.Length > 50)
            {
                Log.Warning($"Ungültiger Vorname={firstName}.");

                firstNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                firstNameInfo.Text = "Vorname darf maximal 50 Zeichen haben.";
                firstNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (!nameRegex.IsMatch(firstName))
            {
                Log.Warning($"Ungültiger Vorname={firstName}.");

                firstNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                firstNameInfo.Text = "Nur Buchstaben, Leerzeichen, - und ' erlaubt.";
                firstNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (firstName.Length > recommendedNameLength)
            {
                Log.Warning($"Ungültiger Vorname={firstName}.");

                firstNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 245, 200));
                firstNameInfo.Text = $"Vorname ist ungewöhnlich lang (max. {recommendedNameLength} empfohlen)";
                firstNameInfo.Foreground = Brushes.DarkOrange;
            }

            string lastName = lastNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(lastName))
            {
                Log.Warning($"Ungültiger Nachname={lastName}.");

                lastNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                lastNameInfo.Text = "Bitte ausfüllen!";
                lastNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (lastName.Length > 50)
            {
                Log.Warning($"Ungültiger Nachname={lastName}.");

                lastNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                lastNameInfo.Text = "Nachname darf maximal 50 Zeichen haben.";
                lastNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (!nameRegex.IsMatch(lastName))
            {
                Log.Warning($"Ungültiger Nachname={lastName}.");

                lastNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                lastNameInfo.Text = "Nur Buchstaben, Leerzeichen, - und ' erlaubt.";
                lastNameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (lastName.Length > recommendedNameLength)
            {
                Log.Warning($"Ungültiger Nachname={lastName}.");

                lastNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 245, 200));
                lastNameInfo.Text = $"Nachname ist ungewöhnlich lang (max. {recommendedNameLength} empfohlen)";
                lastNameInfo.Foreground = Brushes.DarkOrange;
            }

            DateTime? birthDate = birthDatePicker.SelectedDate;
            if (birthDate == null)
            {
                Log.Warning($"Ungültiges Geburtsdatum={birthDate}.");

                birthDatePicker.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                birthDateInfo.Text = "Bitte auswählen!";
                birthDateInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (birthDate < minBirthdate)
            {
                Log.Warning($"Ungültiges Geburtsdatum={birthDate}.");

                birthDatePicker.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                birthDateInfo.Text = "Bitte wähle ein realistisches Geburtsdatum ab dem Jahr 1900.";
                birthDateInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (birthDate > maxBirthdate)
            {
                Log.Warning($"Ungültiges Geburtsdatum={birthDate}.");

                birthDatePicker.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                birthDateInfo.Text = "Du musst mindestens 13 Jahre alt sein.";
                birthDateInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            string email = emailBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                Log.Warning($"Ungültige EMail={email}.");

                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Bitte ausfüllen!";
                emailInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (email.Length > maxEmailLength)
            {
                Log.Warning($"Ungültige EMail={email}.");

                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "E-Mail ist zu lang (maximal 150 Zeichen erlaubt).";
                emailInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (!emailRegex.IsMatch(email))
            {
                Log.Warning($"Ungültige EMail={email}.");

                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Ungültiges Format (z. B. name@domain.com)";
                emailInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            string password = isPasswordVisible ? passwordTextBox.Text.Trim() : passwordBox.Password.Trim();
            if (string.IsNullOrWhiteSpace(password))
            {
                Log.Warning("Ungültiges Passwort.");

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
                Log.Warning("Ungültiges Passwort.");

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
                Log.Warning("Ungültiges Passwort.");

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
                Log.Information("Registrierung abgebrochen wegen Validierungsfehlern in der SignUpPage.");
                return;
            }

            await LoadingAnimation.RunAsync(loginWindow.loadingAnimationCanvas, loginWindow.loadingGrid, async () =>
            {
                bool alreadyExists = await CheckEmailAsync(email);
                if (alreadyExists)
                {
                    Log.Warning($"Registrierung abgebrochen da die EMail={email} bereits vorhanden ist.");

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

                Log.Information($"Sende eine Registrierungsanfrage an Swagger für die EMail={email}.");

                bool success = await api.RegisterAsync(request);
                if (success)
                {
                    Log.Information($"Registrierung erfolgreich für EMail{email}.");

                    var loginResult = await api.LoginAsync(email, password);
                    if (loginResult?.Success == true)
                    {
                        Log.Information($"Login nach Registrierung erfolgreich für die EMail={email}.");
                        loginWindow.GoToMainWindow(loginResult.User, loginResult.Projects);
                    }
                    else
                    {
                        Log.Error("Login nach erfolgreicher Registrierung fehlgeschlagen.");
                        MessageBox.Show("Fehler beim Login.");
                    }
                }
                else
                {
                    Log.Error($"Registrierung fehlgeschlagen für die EMail={email}.");
                    MessageBox.Show("Registrierung fehlgeschlagen. Bitte versuche es erneut.");
                }
            });
        }
    }
}