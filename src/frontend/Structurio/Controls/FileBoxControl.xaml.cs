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
using Structurio.Windows;

namespace Structurio.Controls
{
    /// <summary>
    /// Interaktionslogik für FileBoxControl.xaml
    /// </summary>
    public partial class FileBoxControl : UserControl
    {
        private Brush originalBorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));

        public FileBoxControl(string type, string fileName = "Dateiname")
        {
            InitializeComponent();

            fileNameText.Text = System.IO.Path.GetFileName(fileName);

            Width = 200;
            Height = 250;
        }

        public void SetBackground(Brush brush)
        {
            if (brush is ImageBrush imageBrush && imageBrush.ImageSource != null)
            {
                previewImage.Source = imageBrush.ImageSource;
                imageContainer.Background = Brushes.Transparent;
            }
            else
            {
                imageContainer.Background = brush;
            }
        }

        private void FileBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (previewImage.Source != null)
            {
                var previewWindow = new FilePreviewWindow(previewImage.Source);
                previewWindow.Owner = Window.GetWindow(this);
                previewWindow.ShowDialog();
            }
        }
    }
}