using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
using System.Xml;
using Serilog;
using Structurio.Classes;

namespace Structurio.Pages
{
    /// <summary>
    /// Interaction logic for ProjectDetailPage.xaml
    /// </summary>
    public partial class ProjectDetailPage : Page
    {
        private MainWindow mainWindow;
        private Project project;

        public ProjectDetailPage(MainWindow mainWindow, Project project)
        {
            InitializeComponent();

            Log.Information($"Initialisiere die ProjectDetailPage für das Projekt mit dem Projektnamen={project.Name}.");

            this.project = project;
            this.mainWindow = mainWindow;

            try
            {
                nameText.Text = project.Name.ToUpper();

                contentFrame.Navigate(new KanbanPage(this.project));

                Log.Information($"KanbanPage geladen für das Projekt mit dem Projektnamen={project.Name}.");

                var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(project.Color);
                (nameText.Parent as Border).Background = brush;

                Log.Information($"Farbbox wurde auf die Farbe={project.Color} gesetzt.");
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler beim initialisieren der ProjectDetailPage für Projekt mit dem Projektnamen={project.Name}.");
            }
        }

        private void UncheckAllMenuItems(object sender)
        {
            Log.Information("UncheckAllMenuItems wurde aufgerufen");

            foreach (var child in LogicalTreeHelper.GetChildren(TopbarMenuPanel))
            {
                if (child is ToggleButton button && !ReferenceEquals(button, sender))
                {
                    button.IsChecked = false;
                }
            }
        }

        private void kanban_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            UncheckAllMenuItems(button);

            contentFrame.Navigate(new KanbanPage(this.project));
            button.IsChecked = true;
        }

        private void costs_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            Log.Information("Costs Button wurde geklickt.");
            UncheckAllMenuItems(button);

            contentFrame.Navigate(new Costs(mainWindow.CurrentUser, project));
            button.IsChecked = true;
        }

        private void files_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            Log.Information("Files Button wurde geklickt.");
            UncheckAllMenuItems(button);

            contentFrame.Navigate(new ProjectFoldersPage());
            button.IsChecked = true;
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            Log.Information("Settings Button wurde geklickt.");
            UncheckAllMenuItems(button);

            contentFrame.Navigate(new ProjectSettingsPage(mainWindow, project));
            button.IsChecked = true;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Back Button wurde geklickt in ProjektDetailPage.");

            var mainWindow = Window.GetWindow(this) as MainWindow;

            if (mainWindow != null)
            {
                var projectsPage = new ProjectsPage(mainWindow, mainWindow.UserProjects);
                mainWindow.MainFramePublic.Navigate(projectsPage);
            }
        }
    }
}