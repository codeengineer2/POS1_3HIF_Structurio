using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using Structurio.Windows;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Serilog;

namespace Structurio.Pages
{
    /// <summary>
    /// Repräsentiert die Seite zum Zurücksetzen des Passworts in der Login-Ansicht.
    /// </summary>
    public partial class PasswordResetPage : Page
    {
        private LoginWindow loginWindow;
        private Regex emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

        /// <summary>
        /// Initialisiert die Reset-Page mit Bezug auf das LoginWindow.
        /// </summary>
        /// <param name="loginWindow">Login-Fenster, das die Seite hostet</param>
        public PasswordResetPage(LoginWindow loginWindow)
        {
            InitializeComponent();

            Log.Information("PasswordResetPage wurde geladen.");

            this.loginWindow = loginWindow;
        }

        /// <summary>
        /// Wechsel zurück zur Anmeldeseite.
        /// </summary>
        private void back_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Zurück zur SignInPage gewechselt.");
            loginWindow.GoToSignInPage();
        }

        /// <summary>
        /// Setzt Validierungsanzeige zurück, sobald der Benutzer in das E-Mail-Feld schreibt.
        /// </summary>
        private void emailBox_TextChanged(object sender, RoutedEventArgs e)
        {
            emailBox.Background = Brushes.White;
            emailInfo.Text = "* erforderlich";
            emailInfo.Foreground = Brushes.Gray;
        }

        /// <summary>
        /// Überprüft, ob die angegebene E-Mail im System existiert.
        /// </summary>
        /// <param name="email">E-Mail-Adresse</param>
        /// <returns>true, wenn E-Mail existiert; sonst false</returns>
        private async Task<bool> CheckEmailAsync(string email)
        {
            var mail = new { email };
            var json = JsonConvert.SerializeObject(mail);

            using var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                Log.Information("Überprüfe ob es die EMail={email} schon gibt.");
                var response = await client.PostAsync("http://localhost:8080/auth/check-email", content);
                Log.Information($"StatusCode={response.StatusCode}.");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler bei der EMail Überprüfung von der EMail={email}.");
                return false;
            }
        }

        /// <summary>
        /// Validiert die Eingabe, überprüft die Existenz der E-Mail und zeigt Rückmeldung an.
        /// </summary>
        private async void send_Click(object sender, RoutedEventArgs e)
        {
            int maxEmailLength = 100;
            string email = emailBox.Text.Trim();

            emailBox.Background = Brushes.White;
            emailInfo.Text = "* erforderlich";
            emailInfo.Foreground = Brushes.Gray;

            if (string.IsNullOrWhiteSpace(email))
            {
                Log.Warning($"EMail={email} ist leer.");

                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Bitte E-Mail eingeben!";
                emailInfo.Foreground = Brushes.DarkRed;
                return;
            }

            if (email.Length > maxEmailLength)
            {
                Log.Warning($"EMail={email} ist zu lang.");

                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = $"E-Mail darf maximal {maxEmailLength} Zeichen haben.";
                emailInfo.Foreground = Brushes.DarkRed;
                return;
            }

            if (!emailRegex.IsMatch(email))
            {
                Log.Warning($"Ungültiges EMail Format eingegeben siehe EMail={email}.");

                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Ungültiges Format (z. B. name@domain.com)";
                emailInfo.Foreground = Brushes.DarkRed;
                return;
            }

            Log.Information($"Passwort zurücksetzen wird probiert für die Email={email}.");

            await LoadingAnimation.RunAsync(loginWindow.loadingAnimationCanvas, loginWindow.loadingGrid, async () =>
            {
                bool exists = await CheckEmailAsync(email);
                if (exists)
                {
                    Log.Information($"EMail={email} wurde nicht gefunden.");

                    emailBox.Background = Brushes.LightGreen;
                    emailInfo.Text = "Rücksetzungs-Link wurde (simuliert) gesendet.";
                    emailInfo.Foreground = Brushes.Green;
                }
                else
                {
                    Log.Warning($"EMail={email} wurde nicht gefunden.");

                    emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                    emailInfo.Text = "Es existiert kein Konto mit dieser E-Mail!";
                    emailInfo.Foreground = Brushes.DarkRed;
                }
            });
        }
    }
}