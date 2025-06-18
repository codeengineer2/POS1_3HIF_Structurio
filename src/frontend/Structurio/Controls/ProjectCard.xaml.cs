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

namespace Structurio.Controls
{
    /// <summary>
    /// Interaktionslogik für ProjectCard.xaml
    /// </summary>
    public partial class ProjectCard : UserControl
    {
        public Project Project
        {
            get => (Project)GetValue(ProjectProperty);
            set => SetValue(ProjectProperty, value);
        }
        public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Project), typeof(ProjectCard), new PropertyMetadata(null));
        public event EventHandler Clicked;
        public Brush CardColor
        {
            get
            {
                try
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(Project?.Color ?? "#888"));
                }
                catch
                {
                    return Brushes.Gray;
                }
            }
        }

        public ProjectCard()
        {
            InitializeComponent();
            DataContext = this;
        }
    
        private void ProjectCard_Click(object sender, MouseButtonEventArgs e)
        {
            Log.Information($"ProjectCard wurde geklickt mit Name={Project.Name}.");
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        private void ProjectCard_MouseEnter(object sender, MouseEventArgs e)
        {
            outerBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 245, 245));
        }

        private void ProjectCard_MouseLeave(object sender, MouseEventArgs e)
        {
            outerBorder.Background = Brushes.White;
        }
    }
}