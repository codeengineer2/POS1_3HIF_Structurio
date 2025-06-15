using System;
using System.Text;
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
using System.Windows.Threading;
using Structurio.Pages;
using Structurio.Windows;
using Structurio.Classes;

namespace Structurio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime startTime;
        private DispatcherTimer timer;
        public Frame MainFramePublic;
        public User CurrentUser;
        public List<Project> UserProjects;

        public MainWindow(User user, List<Project> projects)
        {
            InitializeComponent();
            StartTimer();
            this.MainFramePublic = this.mainFrame;
            mainFrame.Navigate(new ProjectsPage(this, projects));
            this.projectsButton.IsChecked = true;
            CurrentUser = user;
            UserProjects = projects;
         
        }

        // übergangs lösung
        public MainWindow()
        {
            InitializeComponent();
            StartTimer();
            this.MainFramePublic = this.mainFrame;

            // testdaten von chatgpt
            var testProjects = new List<Project>
            {
                new Project
                {
                        Id = 1,
                        Name = "HTL",
                        Description = "Redesign der Schulwebseite",
                        Color = "#FF5733",
                        OwnerUid = 1,
                        Board = new Board
                        {
                            Id = 1,
                            Columns = new List<Column>
                            {
                                new Column
                                {
                                    Id = 1,
                                    Name = "Backlog",
                                    Issues = new List<Issue>
                                    {
                                        new Issue { Id = 1, Description = "Erste Aufgabe", ColumnId = 1, Name="HTL"},
                                        new Issue { Id = 2, Description = "Zweite Aufgabe", ColumnId = 1, Name="HTL"}
                                    }
                                },
                                new Column
                                {
                                    Id = 2,
                                    Name = "In Progress",
                                    Issues = new List<Issue>()
                                }
                            }
                        }
                    }
            };

            CurrentUser = null;
            UserProjects = testProjects;
            mainFrame.Navigate(new ProjectsPage(this, testProjects));
            this.projectsButton.IsChecked = true;
            // Window costs = new Costs();
            // costs.Show();
            // Window timestamp = new TimeStamp();
            // timestamp.ShowDialog();
        }


        private void StartTimer()
        {
            startTime = DateTime.Now;

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var elapsed = DateTime.Now - startTime;
            timeText.Text = $"{elapsed:hh\\:mm\\:ss}";
        }

        private void UncheckAllMenuItems(object sender)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(SidebarMenuPanel))
            {
                if (child is ToggleButton button && !ReferenceEquals(button, sender))
                {
                    button.IsChecked = false;
                }
            }
        }

        private void projects_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            UncheckAllMenuItems(button);

            mainFrame.Navigate(new ProjectsPage(this, this.UserProjects));
            button.IsChecked = true;
        }

        private void timestamp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            UncheckAllMenuItems(button);

            mainFrame.Navigate(new TimeStamp(CurrentUser));
            button.IsChecked = true;
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllMenuItems(sender);
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        public void RemoveProject(Project project)
        {
            var removed = UserProjects.Remove(project);
            if (removed)
            {
                MessageBox.Show("Projekt wurde entfernt.");
            }
        }
    }
}