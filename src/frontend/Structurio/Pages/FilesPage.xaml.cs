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
using Structurio.Controls;

namespace Structurio.Pages
{
    /// <summary>
    /// Interaktionslogik für FilesPage.xaml
    /// </summary>
    public partial class FilesPage : Page
    {
        public FilesPage(string type)
        {
            InitializeComponent();

            if (type == "file")
            {
                fileBoxPanel.Children.Add(new FileBoxControl("file"));
                fileBoxPanel.Children.Add(new FileBoxControl("file"));
            }
            else
            {
                fileBoxPanel.Children.Add(new FileBoxControl("diagram"));
                fileBoxPanel.Children.Add(new FileBoxControl("diagram"));
            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            placeholderText.Visibility = string.IsNullOrWhiteSpace(searchBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}