using Serilog;
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
    /// Interaktionslogik für FilePreviewWindow.xaml
    /// </summary>
    public partial class FilePreviewWindow : Window
    {
        public FilePreviewWindow(ImageSource imageSource)
        {
            InitializeComponent();

            if (imageSource != null)
            {
                previewImage.Source = imageSource;
                Log.Information("Dateivorschau wurde geöffnet.");
            }
            else
            {
                Log.Warning("Dateivorschau konnte nicht geöffnet werden.");
                MessageBox.Show("Kein Bild vorhanden.");
                Close();
            }
        }
    }
}