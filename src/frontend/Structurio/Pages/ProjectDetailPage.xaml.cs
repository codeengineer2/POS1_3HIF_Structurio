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
using System.Xml;
using Structurio.Classes;

namespace Structurio.Pages
{
    /// <summary>
    /// Interaction logic for ProjectDetailPage.xaml
    /// </summary>
    public partial class ProjectDetailPage : Page
    {
        private Project project;

        public ProjectDetailPage(Project project)
        {
            InitializeComponent();
            this.project = project;

            nameText.Text = project.Name.ToUpper();
            var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(project.Color);
            (nameText.Parent as Border).Background = brush;
            contentFrame.Navigate(new KanbanPage(project));
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}