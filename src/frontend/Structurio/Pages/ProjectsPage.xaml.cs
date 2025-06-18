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
using Structurio.Controls;
using Structurio.Services;
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

            Log.Information($"Initialisiere die ProjectsPage mit {projects?.Count ?? 0} Projekten.");

            this.mainWindow = mainWindow;
            this.allProjects = projects ?? new List<Project>();

            LoadProjects();
            RenderProjects(allProjectCards);
        }

        private void AddCardToPanel(ProjectCard card)
        {
            card.Clicked += (sender, args) =>
            {
                Log.Information($"Projektkarte wurde geklickt mit dem Projektnamen={card.Project.Name}.");

                var detailPage = new ProjectDetailPage(mainWindow, card.Project);
                mainWindow.MainFramePublic.Navigate(detailPage);
            };

            projectsWrapPanel.Children.Add(card);
        }

        private void LoadProjects()
        {
            Log.Information("Lade alle Projekte in Projektkarten.");

            foreach (var project in allProjects)
            {
                var card = new ProjectCard
                {
                    Project = project
                };
                    
                allProjectCards.Add(card);
                AddCardToPanel(card);
            }
        }

        private void RenderProjects(IEnumerable<ProjectCard> cards)
        {
            Log.Information("Rendert alle Projektkarten und zeigt sie in der UI an.");

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

            Log.Information($"Suche wurde gestartet mit dem Query={query}.");

            RenderProjects(filtered);
        }

        private async void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("CreateProjectButton wurde geklickt.");

            var window = new CreateProjectWindow();
            window.Owner = Window.GetWindow(this);

            if (window.ShowDialog() != true)
            {
                Log.Information("Die Projekterstellung wurde vom Benutzer abgebrochen.");
                return;
            }

            var request = new ProjectRequest
            {
                Name = window.ProjectName,
                Description = window.ProjectDescription,
                Color = window.ProjectColor,
                OwnerUid = mainWindow.CurrentUser.Id
            };

            Log.Information($"Sende ein neues Projekt zu Swagger mit dem Namen={request.Name}.");

            var api = new ApiService();

            await LoadingAnimation.RunAsync(loadingCanvas, loadingGrid, async () =>
            {
                var newProject = await api.CreateProjectAsync(request);
                if (newProject != null)
                {
                    Log.Information($"Projekt wurde erfolgreich erstellt mit dem Namen={newProject.Name}.");

                    var card = new ProjectCard { Project = newProject };

                    allProjects.Add(newProject);
                    allProjectCards.Add(card);

                    AddCardToPanel(card);
                }
                else
                {
                    Log.Error("Projekt konnte nicht erstellt werden.");
                    MessageBox.Show("Projekt konnte nicht erstellt werden!");
                }
            });
        }
    }
}