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

        private User currentUser;
        private List<Project> userProjects;

        public MainWindow(User user, List<Project> projects)
        {
            InitializeComponent();
            StartTimer();
            this.MainFramePublic = this.mainFrame;
            mainFrame.Navigate(new ProjectsPage(this, projects));
            this.projectsButton.IsChecked = true;
            currentUser = user;
            userProjects = projects;
            // Window costs = new Costs();
            // costs.Show();
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
                    Board = new Board()
                },
                new Project
                {
                    Id = 2,
                    Name = "AM",
                    Description = "Projekt zur Matura-Präsentation",
                    Color = "#3498DB",
                    OwnerUid = 1,
                    Board = new Board()
                }
            };

            mainFrame.Navigate(new ProjectsPage(this, testProjects));
            this.projectsButton.IsChecked = true;
            currentUser = null;
            userProjects = testProjects;
            // Window costs = new Costs();
            // costs.Show();
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
                if (child is ToggleButton btn && !ReferenceEquals(btn, sender))
                {
                    btn.IsChecked = false;
                }
            }
        }

        private void projects_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllMenuItems(sender);
            mainFrame.Navigate(new ProjectsPage(this, this.userProjects));
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
    }
}