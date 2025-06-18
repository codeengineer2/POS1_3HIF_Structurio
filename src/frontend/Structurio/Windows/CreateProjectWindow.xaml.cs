using Serilog;
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

namespace Structurio.Windows
{
    /// <summary>
    /// Fenster zur Erstellung eines neuen Projekts.
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        /// <summary>
        /// Projektname (max. 5 Zeichen, Pflichtfeld).
        /// </summary>
        public string ProjectName { get; private set; }

        /// <summary>
        /// Projektbeschreibung (max. 200 Zeichen, Pflichtfeld).
        /// </summary>
        public string ProjectDescription { get; private set; }

        /// <summary>
        /// Farbwert des Projekts als Hex-String.
        /// </summary>
        public string ProjectColor { get; private set; }

        /// <summary>
        /// Initialisiert das Fenster.
        /// </summary>
        public CreateProjectWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prüft Eingaben und erstellt bei Gültigkeit das Projekt.
        /// </summary>
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            string nameText = nameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(nameText))
            {
                nameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                nameInfo.Text = "Name ist erforderlich!";
                nameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (nameText.Length > 5)
            {
                nameBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                nameInfo.Text = "Name darf max. 5 Zeichen haben!";
                nameInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else
            {
                nameBox.ClearValue(BackgroundProperty);
                nameInfo.Text = "* erforderlich";
                nameInfo.Foreground = Brushes.Gray;
            }

            string descriptionText = descriptionBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(descriptionText))
            {
                descriptionBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                descriptionInfo.Text = "Beschreibung ist erforderlich!";
                descriptionInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else if (descriptionText.Length > 200)
            {
                descriptionBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                descriptionInfo.Text = "Beschreibung darf max. 200 Zeichen haben!";
                descriptionInfo.Foreground = Brushes.DarkRed;
                valid = false;
            }
            else
            {
                descriptionBox.ClearValue(BackgroundProperty);
                descriptionInfo.Text = "* erforderlich";
                descriptionInfo.Foreground = Brushes.Gray;
            }

            if (!valid)
            {
                Log.Warning("Projekt konnte nicht erstellt werden.");
                return;
            }

            ProjectName = nameText;
            ProjectDescription = descriptionText;
            ProjectColor = (colorPicker.SelectedColor ?? Colors.LightGray).ToString();

            Log.Information($"Neues Projekt wurde erstellt mit dem Namen={ProjectName}.");

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Setzt Namensfeld-Fehlermeldung zurück.
        /// </summary>
        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            nameBox.ClearValue(BackgroundProperty);
            nameInfo.Text = "* erforderlich";
            nameInfo.Foreground = Brushes.Gray;
        }

        /// <summary>
        /// Setzt Beschreibungsfeld-Fehlermeldung zurück.
        /// </summary>
        private void DescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            descriptionBox.ClearValue(BackgroundProperty);
            descriptionInfo.Text = "* erforderlich";
            descriptionInfo.Foreground = Brushes.Gray;
        }
    }
}