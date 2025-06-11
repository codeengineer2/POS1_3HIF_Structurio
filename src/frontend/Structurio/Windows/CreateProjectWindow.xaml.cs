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
using System.Windows.Shapes;

namespace Structurio.Windows
{
    /// <summary>
    /// Interaktionslogik für CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        public string ProjectName { get; private set; }
        public string ProjectDescription { get; private set; }
        public string ProjectColor { get; private set; }

        public CreateProjectWindow()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameBox.Text))
            {
                MessageBox.Show("Error!");
                return;
            }

            ProjectName = nameBox.Text.Trim();
            ProjectDescription = descriptionBox.Text.Trim();

            var selectedColor = colorPicker.SelectedColor ?? Colors.LightGray;
            ProjectColor = selectedColor.ToString();

            DialogResult = true;
            Close();
        }
    }   
}