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

namespace Structurio.Controls
{
    /// <summary>
    /// Interaktionslogik für FileBoxControl.xaml
    /// </summary>
    public partial class FileBoxControl : UserControl
    {
        private Brush originalBorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));

        public FileBoxControl(string type)
        {
            InitializeComponent();

            if (type.ToLower().Contains("file"))
            {
                Width = 150; 
                Height = 200;
            }
            else
            {
                Width = 200; 
                Height = 200;
            }
        }

        private void FileBox_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Hallo!");
        }

        private void FileBox_MouseEnter(object sender, MouseEventArgs e)
        {
            outerBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            outerBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
        }

        private void FileBox_MouseLeave(object sender, MouseEventArgs e)
        {
            outerBorder.Background = Brushes.White;
            outerBorder.BorderBrush = originalBorderBrush;
        }
    }
}