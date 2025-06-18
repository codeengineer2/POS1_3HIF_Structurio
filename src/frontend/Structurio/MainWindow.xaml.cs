using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Structurio.Pages;
using Structurio.Windows;
using Structurio.Classes;
using Serilog;
using Serilog.Core;

namespace Structurio
{
    /// <summary>
    /// Hauptfenster für die Benutzeroberfläche.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Startzeit für den Timer.
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// DispatcherTimer für Laufzeitanzeige.
        /// </summary>
        private DispatcherTimer timer;

        /// <summary>
        /// Öffentlicher Zugriff auf das MainFrame.
        /// </summary>
        public Frame MainFramePublic;

        /// <summary>
        /// Aktuell eingeloggter Benutzer.
        /// </summary>
        public User CurrentUser;

        /// <summary>
        /// Liste der Benutzerprojekte.
        /// </summary>
        public List<Project> UserProjects;

        /// <summary>
        /// Konstruktor im Livebetrieb mit Userdaten.
        /// </summary>
        public MainWindow(User user, List<Project> projects)
        {
            InitializeComponent();

            StartTimer();

            Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("log.txt", rollingInterval: RollingInterval.Day).CreateLogger();
            Log.Information($"MainWindow wurde gestartet vom Benutzer mit Email={CurrentUser.Email} und mit {projects.Count} Projekten.");

            this.MainFramePublic = this.mainFrame;
            mainFrame.Navigate(new ProjectsPage(this, projects));

            CurrentUser = user;
            UserProjects = projects;

            this.projectsButton.IsChecked = true;
        }

        /// <summary>
        /// Konstruktor für Entwicklermodus mit Testdaten.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            StartTimer();

            Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("log.txt", rollingInterval: RollingInterval.Day).CreateLogger();
            Log.Warning("MainWindow wurde im Entwicklermodus gestartet.");

            var testProjects = new List<Project>
            {
                new Project
                {
                        Id = 1,
                        Name = "HTL",
                        Description = "Besseres Design der Schulwebseite",
                        Color = "#FF5733",
                        OwnerUid = 1,
                        Board = new Board
                        {
                            Id = 1,
                            Columns = new List<Column>
                            {
                                new Column
                                {
                                    Id = 1,
                                    Name = "Aufgaben",
                                    Issues = new List<Issue>
                                    {
                                        new Issue { Id = 1, Description = "Aufgabe1", ColumnId = 1, Name="HTL"},
                                        new Issue { Id = 2, Description = "Aufgabe2", ColumnId = 1, Name="HTL"}
                                    }
                                },
                                new Column
                                {
                                    Id = 2,
                                    Name = "Erledigt",
                                    Issues = new List<Issue>()
                                }
                            }
                        }
                    }
            };

            this.MainFramePublic = this.mainFrame;
            mainFrame.Navigate(new ProjectsPage(this, testProjects));

            CurrentUser = null;
            UserProjects = testProjects;

            this.projectsButton.IsChecked = true;
        }

        /// <summary>
        /// Startet den Zeitmesser.
        /// </summary>
        private void StartTimer()
        {
            startTime = DateTime.Now;

            Log.Debug($"Timer wurde gestartet: {startTime}");

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += timer_Tick;
            timer.Start();
        }

        /// <summary>
        /// Aktualisiert die Zeitdarstellung.
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - startTime;
            timeText.Text = $"{elapsed:hh\\:mm\\:ss}";
        }

        /// <summary>
        /// Deaktiviert alle Menübuttons außer dem aktiven.
        /// </summary>
        private void UncheckAllMenuItems(object sender)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(SidebarMenuPanel))
            {
                if (child is ToggleButton button && !ReferenceEquals(button, sender))
                {
                    button.IsChecked = false;
                }
            }
        }

        /// <summary>
        /// Öffnet die Projektübersicht.
        /// </summary>
        private void projects_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            UncheckAllMenuItems(button);

            Log.Information($"Benutzer hat auf das Menü Element namens Projekte geklickt.");

            mainFrame.Navigate(new ProjectsPage(this, this.UserProjects));
            button.IsChecked = true;
        }

        /// <summary>
        /// Öffnet die Zeitstempelansicht.
        /// </summary>
        private void timestamp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            UncheckAllMenuItems(button);

            Log.Information("Benutzer hat auf das Menü Element namens Zeitstempeln geklickt.");

            mainFrame.Navigate(new TimeStamp(CurrentUser));
            button.IsChecked = true;
        }

        /// <summary>
        /// Öffnet die Einstellungen (noch leer).
        /// </summary>
        private void settings_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllMenuItems(sender);
        }

        /// <summary>
        /// Führt Logout durch und öffnet das Loginfenster.
        /// </summary>
        private void logout_Click(object sender, RoutedEventArgs e)
        {
            Log.Information($"Benutzer mit Email={CurrentUser.Email} hat sich abgemeldet.");

            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Entfernt ein Projekt anhand der ID.
        /// </summary>
        public void RemoveProjectById(int pid)
        {
            var projectToRemove = UserProjects.FirstOrDefault(p => p.Id == pid);
            if (projectToRemove != null)
            {
                Log.Information($"Projekt entfernt: ID={pid}, Name={projectToRemove}.");
                UserProjects.Remove(projectToRemove);
                mainFrame.Navigate(new ProjectsPage(this, UserProjects));
            }
            else
            {
                Log.Error($"Projekt mit ID={pid} nicht gefunden.");
            }
        }
    }
}