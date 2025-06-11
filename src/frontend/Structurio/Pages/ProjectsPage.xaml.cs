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
using Structurio.Windows;

namespace Structurio.Pages
{
    public partial class ProjectsPage : Page
    {
        private MainWindow mainWindow;
        private List<Project> allProjects;
        private List<ProjectCard> allProjectCards = new ();

        public ProjectsPage(MainWindow mainWindow, List<Project> projects)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.allProjects = projects ?? new List<Project>();

            LoadProjects();
            RenderProjects(allProjectCards);
        }

        private void AddCardToPanel(ProjectCard card)
        {
            card.Clicked += (sender, args) =>
            {
                var detailPage = new ProjectDetailPage(card.Project);
                mainWindow.MainFramePublic.Navigate(detailPage);
            };

            projectsWrapPanel.Children.Add(card);
        }

        private void LoadProjects()
        {

            foreach (var project in allProjects)
            {
                var card = new ProjectCard
                {
                    Project = project
                };

                allProjectCards.Add(card);
            }

            RenderProjects(allProjectCards);
        }

        private void RenderProjects(IEnumerable<ProjectCard> cards)
        {
            projectsWrapPanel.Children.Clear();

            foreach (var card in cards)
            {
                AddCardToPanel(card);
            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            placeholderText.Visibility = string.IsNullOrWhiteSpace(searchBox.Text) ? Visibility.Visible : Visibility.Collapsed;

            var query = searchBox.Text.ToLower();
            var filtered = allProjectCards.Where(p => p.Project.Name.ToLower().Contains(query));

            RenderProjects(filtered);
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CreateProjectWindow();
            window.Owner = Window.GetWindow(this);

            if (window.ShowDialog() == true)
            {
                var newProject = new Project
                {
                    Name = window.ProjectName,
                    Description = window.ProjectDescription,
                    Color = window.ProjectColor
                };

                var card = new ProjectCard { Project = newProject };
                allProjects.Add(newProject);
                allProjectCards.Add(card);
                AddCardToPanel(card);
            }
        }
    }
}