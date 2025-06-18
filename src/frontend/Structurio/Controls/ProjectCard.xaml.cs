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
    /// UI-Karte für ein Projekt in der Übersicht.
    /// </summary>
    public partial class ProjectCard : UserControl
    {
        /// <summary>
        /// Bindbares Projektobjekt.
        /// </summary>
        public Project Project
        {
            get => (Project)GetValue(ProjectProperty);
            set => SetValue(ProjectProperty, value);
        }

        /// <summary>
        /// DependencyProperty für Projektbindung.
        /// </summary>
        public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Project), typeof(ProjectCard), new PropertyMetadata(null));

        /// <summary>
        /// Event beim Klick auf die Karte.
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// Gibt die Farbe der Karte zurück (aus dem Projekt).
        /// </summary>
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

        /// <summary>
        /// Erstellt eine neue ProjectCard.
        /// </summary>
        public ProjectCard()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Wird ausgelöst beim Klick auf die Karte.
        /// </summary>
        private void ProjectCard_Click(object sender, MouseButtonEventArgs e)
        {
            Log.Information($"ProjectCard wurde geklickt mit Name={Project.Name}.");
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Hover-Effekt beim Betreten.
        /// </summary>
        private void ProjectCard_MouseEnter(object sender, MouseEventArgs e)
        {
            outerBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
        }

        /// <summary>
        /// Hover-Effekt beim Verlassen.
        /// </summary>
        private void ProjectCard_MouseLeave(object sender, MouseEventArgs e)
        {
            outerBorder.Background = Brushes.White;
        }
    }
}