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

namespace Structurio.Pages
{
    /// <summary>
    /// Interaktionslogik für PasswordResetPage.xaml
    /// </summary>
    public partial class PasswordResetPage : Page
    {
        private LoginWindow loginWindow;
        private Regex emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

        public PasswordResetPage(LoginWindow loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            loginWindow.GoToSignInPage();
        }

        private void emailBox_TextChanged(object sender, RoutedEventArgs e)
        {
            emailBox.Background = Brushes.White;
            emailInfo.Text = "* erforderlich";
            emailInfo.Foreground = Brushes.Gray;
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

        private async void send_Click(object sender, RoutedEventArgs e)
        {
            int maxEmailLength = 100;

            string email = emailBox.Text.Trim();

            emailBox.Background = Brushes.White;
            emailInfo.Text = "* erforderlich";
            emailInfo.Foreground = Brushes.Gray;

            if (string.IsNullOrWhiteSpace(email))
            {
                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Bitte E-Mail eingeben!";
                emailInfo.Foreground = Brushes.DarkRed;
                return;
            }

            if (email.Length > maxEmailLength)
            {
                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = $"E-Mail darf maximal {maxEmailLength} Zeichen haben.";
                emailInfo.Foreground = Brushes.DarkRed;
                return;
            }

            if (!emailRegex.IsMatch(email))
            {
                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Ungültiges Format (z. B. name@domain.com)";
                emailInfo.Foreground = Brushes.DarkRed;
                return;
            }

            await LoadingAnimation.RunAsync(loginWindow.loadingAnimationCanvas, loginWindow.loadingGrid, async () =>
            {
                bool exists = await CheckEmailAsync(email);
                if (exists)
                {
                    emailBox.Background = Brushes.LightGreen;
                    emailInfo.Text = "Rücksetzungs-Link wurde (simuliert) gesendet.";
                    emailInfo.Foreground = Brushes.Green;
                }
                else
                {
                    emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                    emailInfo.Text = "Es existiert kein Konto mit dieser E-Mail!";
                    emailInfo.Foreground = Brushes.DarkRed;
                }
            });
        }
    }
}