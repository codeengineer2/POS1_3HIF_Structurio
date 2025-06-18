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
using Serilog;

namespace Structurio.Windows
{
    /// <summary>
    /// Fenster zur Benutzeranmeldung.
    /// </summary>
    public partial class LoginWindow : Window
    {
        private IApiService api = new ApiService();

        /// <summary>
        /// Initialisiert das Login-Fenster und zeigt die Anmeldeseite.
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();

            Log.Information("LoginWindow wurde geöffnet.");

            loginFrame.Navigate(new SignInPage(this, api));
        }

        /// <summary>
        /// Öffnet die Hauptanwendung nach erfolgreichem Login.
        /// </summary>
        /// <param name="user">Eingeloggter Benutzer.</param>
        /// <param name="projects">Zugehörige Projekte.</param>
        public void GoToMainWindow(User user, List<Project> projects)
        {
            Log.Information($"Erfolgreich eingeloggt als Benutzer mit der Email={user.Email} und mit {projects.Count} Projekten.");

            var mainWindow = new MainWindow(user, projects);
            mainWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Navigiert zur Seite für Passwort-Zurücksetzung.
        /// </summary>
        public void GoToPasswordResetPage()
        {
            Log.Information("Navigiert zur PasswordResetPage.");
            loginFrame.Navigate(new PasswordResetPage(this));
        }

        /// <summary>
        /// Navigiert zur Anmeldeseite.
        /// </summary>
        public void GoToSignInPage()
        {
            Log.Information("Navigiert zur SignInPage.");
            loginFrame.Navigate(new SignInPage(this, api));
        }

        /// <summary>
        /// Navigiert zur Registrierungsseite.
        /// </summary>
        public void GoToSignUpPage()
        {
            Log.Information("Navigiert zur SignUpPage.");
            loginFrame.Navigate(new SignUpPage(this, api));
        }
    }
}