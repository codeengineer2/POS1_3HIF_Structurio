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
            
        }
    }
}