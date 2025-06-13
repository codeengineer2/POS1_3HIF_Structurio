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
        private List<FolderBoxControl> allFolderBoxes = new();

        public ProjectFoldersPage()
        {
            InitializeComponent();

            var folder1 = new FolderBoxControl("file");
            var folder2 = new FolderBoxControl("diagram");

            allFolderBoxes.Add(folder1);
            allFolderBoxes.Add(folder2);

            filesWrapPanel.Children.Add(folder1);
            filesWrapPanel.Children.Add(folder2);
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            placeholderText.Visibility = string.IsNullOrWhiteSpace(searchBox.Text) ? Visibility.Visible : Visibility.Collapsed;

            var query = searchBox.Text.ToLower();
            var filtered = allFolderBoxes.Where(f => f.Name.Contains(query)).ToList();

            filesWrapPanel.Children.Clear();
            foreach (var folder in filtered)
            {
                filesWrapPanel.Children.Add(folder);
            }
        }
    }
}