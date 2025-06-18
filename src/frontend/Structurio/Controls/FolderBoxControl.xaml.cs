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
using Structurio.Pages;

namespace Structurio.Controls
{
    /// <summary>
    /// Interaktionslogik für FolderBoxControl.xaml
    /// </summary>
    public partial class FolderBoxControl : UserControl
    {
        private Brush originalBorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));
        private string type;
        public string Name;

        public FolderBoxControl(string title)
        {
            InitializeComponent();

            type = title.ToLower().Contains("file") ? "file" : "diagram";
            titleText.Text = type == "file" ? "📁 Dateien" : "📊 Diagramme";
            this.Name = titleText.Text.ToLower();
            Log.Information($"Ordner erstellt mit Typ={type}.");
        }

        private void FolderBox_Click(object sender, MouseButtonEventArgs e)
        {
            Log.Information("Benutzer klickt auf den Ordner.");
            NavigationService.GetNavigationService(this)?.Navigate(new FilesPage(type));
        }

        private void FolderBox_MouseEnter(object sender, MouseEventArgs e)
        {
            outerBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            outerBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
        }

        private void FolderBox_MouseLeave(object sender, MouseEventArgs e)
        {
            outerBorder.Background = Brushes.White;
            outerBorder.BorderBrush = originalBorderBrush;
        }
    }
}