using Structurio.Windows;
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

namespace Structurio.Pages
{
    /// <summary>
    /// Interaktionslogik für PasswordResetPage.xaml
    /// </summary>
    public partial class PasswordResetPage : Page
    {
        private LoginWindow loginWindow;

        public PasswordResetPage(LoginWindow loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            loginWindow.GoToSignInPage();
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            emailBox.Background = Brushes.White;
            emailInfo.Text = "* erforderlich";
            emailInfo.Foreground = Brushes.Gray;

            if (string.IsNullOrWhiteSpace(emailBox.Text))
            {
                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Bitte E-Mail eingeben!";
                emailInfo.Foreground = Brushes.DarkRed;
                return;
            }

            if (emailBox.Text.ToLower() != "a") // benutze 'a' für test email
            {
                emailBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                emailInfo.Text = "Es existiert kein Konto mit dieser E-Mail!";
                emailInfo.Foreground = Brushes.DarkRed;
            }
            else
            {
                emailBox.Background = Brushes.LightGreen;
                emailInfo.Text = "Rücksetzungs-Link wurde (simuliert) gesendet.";
                emailInfo.Foreground = Brushes.Green;
            }
        }

        private void emailBox_TextChanged(object sender, RoutedEventArgs e)
        {
            emailBox.Background = Brushes.White;
            emailInfo.Text = "* erforderlich";
            emailInfo.Foreground = Brushes.Gray;
        }
    }
}