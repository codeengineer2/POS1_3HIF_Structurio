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
using System.Windows.Shapes;
using Serilog;
using Structurio.Classes;

namespace Structurio.Windows
{
    /// <summary>
    /// Fenster zum Bearbeiten oder Löschen eines bestehenden Issues.
    /// </summary>
    public partial class UpdateIssueWindow : Window
    {
        /// <summary>Aktualisierte Beschreibung des Issues.</summary>
        public string UpdatedDescription { get; private set; }

        /// <summary>Gibt an, ob das Issue gelöscht wurde.</summary>
        public bool IsDeleted { get; private set; } = false;

        private Issue issue;

        /// <summary>
        /// Initialisiert das Fenster mit einem bestehenden Issue.
        /// </summary>
        /// <param name="issue">Das zu bearbeitende Issue.</param>
        public UpdateIssueWindow(Issue issue)
        {
            InitializeComponent();

            this.issue = issue;
            descriptionBox.Text = issue.Description;

            Log.Information($"UpdateIssueWindow wurde geöffnet für das Issue mit der Beschreibung={issue.Description}");
        }

        /// <summary>
        /// Speichert die aktualisierte Beschreibung, wenn gültig.
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var text = descriptionBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                descriptionBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                descriptionInfo.Text = "Beschreibung ist erforderlich!";
                descriptionInfo.Foreground = Brushes.DarkRed;
                return;
            }
            if (text.Length > 100)
            {
                descriptionBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                descriptionInfo.Text = "Maximal 100 Zeichen erlaubt!";
                descriptionInfo.Foreground = Brushes.DarkRed;
                return;
            }

            Log.Information($"Beschreibung von dem Issue mit der Beschreibung={text} wurde erfolgreich aktualisiert.");

            UpdatedDescription = text;
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Löscht das Issue.
        /// </summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Log.Warning($"Das Issue mit der Beschreibung={issue.Description} wurde gelöscht.");

            IsDeleted = true;
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Setzt die Fehleranzeige beim Ändern des Beschreibungstextes zurück.
        /// </summary>
        private void DescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            descriptionBox.ClearValue(BackgroundProperty);
            descriptionInfo.Text = "* erforderlich";
            descriptionInfo.Foreground = Brushes.Gray;
        }
    }
}