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
using System.IO;
using Serilog;

namespace Structurio.Controls
{
    /// <summary>
    /// Interaktionslogik für FileBoxControl.xaml
    /// </summary>
    public partial class FileBoxControl : UserControl
    {
        private Brush originalBorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));
        private string filePath;
        public string FileName;

        public FileBoxControl(string type, string fileName = "Dateiname", string path = "")
        {
            InitializeComponent();

            filePath = path;
            fileNameText.Text = System.IO.Path.GetFileName(fileName);
            FileName = fileNameText.Text.ToLower();

            Width = 200;
            Height = 250;

            Log.Information($"Datei erstellt mit Dateiname={FileName} und Pfad={filePath}.");
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
                Log.Information($"Datei (in Edge wenn vorhanden) geöffnet mit Dateiname={FileName}.");
                var previewWindow = new FilePreviewWindow(previewImage.Source);
                previewWindow.Owner = Window.GetWindow(this);
                previewWindow.ShowDialog();
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Log.Information($"Versuch die Datei zu öffnen mit Pfad={filePath}");

                if (File.Exists(filePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath)
                    {
                        UseShellExecute = true
                    });
                }
                else
                {
                    Log.Warning($"Datei wurde nicht gefunden mit Pfad={filePath}");
                    MessageBox.Show("Datei wurde nicht gefunden!");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler beim öffnen der Datei mit Pfad = {filePath}");
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Möchten Sie diese Datei wirklich löschen?", "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Log.Information($"Datei wurde gelöscht mit Dateinamen=FileName {FileName}.");
                var parent = this.Parent as Panel;
                parent?.Children.Remove(this);
            }
        }
    }
}