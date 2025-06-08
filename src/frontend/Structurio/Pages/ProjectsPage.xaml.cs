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
    public partial class ProjectsPage : Page
    {
        private MainWindow mainWindow;
        private List<Project> allProjects;
        private List<ProjectCard> allProjectCards;

        public ProjectsPage(MainWindow mainWindow, List<Project> projects)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.allProjects = projects ?? new List<Project>();
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}