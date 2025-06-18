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
using Serilog;
using Structurio.Classes;
using Structurio.Interfaces;
using Structurio.Services;
using static OpenTK.Graphics.OpenGL.GL;

namespace Structurio.Pages
{
    /// <summary>
    /// Detaileinstellungen für ein Projekt (Name, Beschreibung, Farbe, Löschen).
    /// </summary>
    public partial class ProjectSettingsPage : Page
    {
        private MainWindow mainWindow;
        private IApiService apiService;
        private Project project;

        /// <summary>
        /// Initialisiert die ProjectSettingsPage mit Projekt und MainWindow.
        /// </summary>
        public ProjectSettingsPage(MainWindow mainWindow, Project project)
        {
            InitializeComponent();

            Log.Information($"Initialisiere die ProjectSettingsPage für Projekt das Projekt mit dem Namen={project.Name}.");

            this.mainWindow = mainWindow;
            this.project = project;

            nameBox.Text = project.Name;
            descriptionBox.Text = project.Description;
            colorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(project.Color);
        }

        /// <summary>
        /// Setzt Fehlermarkierungen beim Namensfeld zurück.
        /// </summary>
        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            nameBox.ClearValue(BackgroundProperty);
            nameInfo.Text = "* erforderlich";
            nameInfo.Foreground = Brushes.Gray;
        }

        /// <summary>
        /// Setzt Fehlermarkierungen beim Beschreibungsfeld zurück.
        /// </summary>
        private void DescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            descriptionBox.ClearValue(BackgroundProperty);
            descriptionInfo.Text = "* erforderlich";
            descriptionInfo.Foreground = Brushes.Gray;
        }

        /// <summary>
        /// Löscht das aktuelle Projekt nach Bestätigung.
        /// </summary>
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Log.Information($"Delete Button wurde geklickt für Projekt mit dem Namen={project.Name}");

            if (mainWindow.CurrentUser.Id != project.OwnerUid)
            {
                Log.Warning("Löschversuch fehlgeschlagen weil du nicht der Eigentümer des Projekts bist.");
                MessageBox.Show("Du darfst dieses Projekt nicht löschen. Es gehört dir nicht.");
                return;
            }

            var confirmed = MessageBox.Show("Wirklich löschen?", "Bestätigen", MessageBoxButton.YesNo);
            if (confirmed != MessageBoxResult.Yes)
            {
                Log.Information("Löschung vom Benutzer abgebrochen.");
                return;
            }

            var api = new ApiService();

            await LoadingAnimation.RunAsync(loadingCanvas, loadingGrid, async () =>
            {
                bool success = await api.DeleteProjectAsync(project.Id);
                if (success)
                {
                    Log.Information($"Projekt mit dem Namen={project.Name} wurde erfolgreich gelöscht.");
                    mainWindow.RemoveProjectById(project.Id);
                }
                else
                {
                    Log.Error($"Fehler beim Löschen des Projekts mit dem Namen={project.Name}.");
                    MessageBox.Show("Fehler beim Löschen");
                }
            });
        }

        /// <summary>
        /// Validiert und speichert Änderungen am Projekt, wenn der Benutzer der Besitzer ist.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Log.Information($"Save Button wurde geklickt für das Projekt mit dem Namen={project.Name}.");

            if (mainWindow.CurrentUser.Id != project.OwnerUid)
            {
                Log.Warning($"Benutzer mit der ID={mainWindow.CurrentUser.Id} versucht das Projekt mit dem Namen={project.Name} zu ändern obwohl er nicht der Besitzer ist.");
                MessageBox.Show("Sie sind nicht der Besitzer dieses Projekts. Änderungen sind daher nicht erlaubt.");
                return;
            }

            bool valid = true;

            string nameText = nameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(nameText))
            {
                Log.Warning($"Name={project.Name} ist leer.");

                nameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                nameInfo.Text = "Name ist erforderlich!";
                nameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (nameText.Length > 5)
            {
                Log.Warning($"Name={project.Name} ist zu lang.");

                nameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                nameInfo.Text = "Name darf max. 5 Zeichen haben!";
                nameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            string descriptionText = descriptionBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(descriptionText))
            {
                Log.Warning($"Beschreibung={project.Description} ist leer.");

                descriptionBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                descriptionInfo.Text = "Beschreibung ist erforderlich!";
                descriptionInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (descriptionText.Length > 200)
            {
                Log.Warning($"Beschreibung={project.Description} ist zu lang.");

                descriptionBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                descriptionInfo.Text = "Beschreibung darf max. 200 Zeichen haben!";
                descriptionInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }

            if (!valid)
            {
                Log.Information("Speichern abgebrochen wegen Validierungsfehlern in der ProjectSettingsPage.");
                return;
            }

            project.Name = nameText;
            project.Description = descriptionText;
            project.Color = (colorPicker.SelectedColor ?? Colors.LightGray).ToString();

            await LoadingAnimation.RunAsync(loadingCanvas, loadingGrid, async () =>
            {
                var api = new ApiService();

                bool success = await api.UpdateProjectAsync(project);
                if (success)
                {
                    Log.Information($"Projekt mit dem Namen={project.Name} wurde erfolgreich aktualisiert.");
                    mainWindow.MainFramePublic.Navigate(new ProjectsPage(mainWindow, mainWindow.UserProjects));
                }
                else
                {
                    Log.Error($"Fehler beim Aktualisieren des Projekts mit dem Namen={project.Name}.");
                    MessageBox.Show("Fehler beim Speichern des Projekts.");
                }
            });
        }
    }
}