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
    /// Interaktionslogik für ProjectFoldersPage.xaml
    /// </summary>
    public partial class ProjectFoldersPage : Page
    {
        public ProjectFoldersPage()
        {
            InitializeComponent();
            filesWrapPanel.Children.Add(new FolderBoxControl("Dateien"));
            filesWrapPanel.Children.Add(new FolderBoxControl("Diagramme"));
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            placeholderText.Visibility = string.IsNullOrWhiteSpace(searchBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}