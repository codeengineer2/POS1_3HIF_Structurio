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
using Structurio.Classes;

namespace Structurio.Pages
{
    /// <summary>
    /// Interaktionslogik für ProjectSettingsPage.xaml
    /// </summary>
    public partial class ProjectSettingsPage : Page
    {
        private MainWindow mainWindow;
        private Project project;

        public ProjectSettingsPage(MainWindow mainWindow, Project project)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.project = project;
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            nameBox.ClearValue(BackgroundProperty);
            nameInfo.Text = "* erforderlich";
            nameInfo.Foreground = Brushes.Gray;
        }

        private void DescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            descriptionBox.ClearValue(BackgroundProperty);
            descriptionInfo.Text = "* erforderlich";
            descriptionInfo.Foreground = Brushes.Gray;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this);

            var result = MessageBox.Show(owner, $"Möchtest du das Projekt „{project.Name}“ wirklich entfernen?", "Projekt entfernen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                mainWindow.RemoveProject(project);
                mainWindow.MainFramePublic.Navigate(new ProjectsPage(mainWindow, mainWindow.UserProjects));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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
                return;
            }

            project.Name = nameText;
            project.Description = descriptionText;
            project.Color = (colorPicker.SelectedColor ?? Colors.LightGray).ToString();

            mainWindow.MainFramePublic.Navigate(new ProjectsPage(mainWindow, mainWindow.UserProjects));
        }
    }
}