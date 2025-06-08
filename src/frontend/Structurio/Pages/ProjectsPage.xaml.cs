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
using Structurio.Controls;

namespace Structurio.Pages
{
    /// <summary>
    /// Interaktionslogik für ProjectsPage.xaml
    /// </summary>
    public partial class ProjectsPage : Page
    {
        private List<Project> allProjectsBackend;
        private List<ProjectCard> allProjectsFrontend;
        private MainWindow mainWindow;

        public ProjectsPage(MainWindow mainWindow, List<Project> projects)
        {
            InitializeComponent();
            mainWindow = mainWindow;
            allProjectsBackend = projects;
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}