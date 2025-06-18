using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using Serilog;
using Structurio.Controls;
    
namespace Structurio.Pages
{
    /// <summary>
    /// Interaktionslogik für FilesPage.xaml
    /// </summary>
    public partial class FilesPage : Page
    {
        private List<FileBoxControl> allFileBoxes = new();

        public FilesPage(string type)
        {
            InitializeComponent();

            string projectPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            string path = "";

            if (type == "file")
            {
                Log.Information("Dateityp ist 'file'.");

                path = System.IO.Path.Combine(projectPath, "file.pdf");
                var fileBox = new FileBoxControl("file", "file.pdf", path);
                AddFileBoxFromPath(path);
            }
            else
            {
                Log.Information("Dateityp ist 'diagram'.");

                path = System.IO.Path.Combine(projectPath, "diagram.pdf");
                var fileBox = new FileBoxControl("diagram", "diagram.pdf", path);
                AddFileBoxFromPath(path);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Back Button wurde geklickt.");
            NavigationService?.Navigate(new ProjectFoldersPage());
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            placeholderText.Visibility = string.IsNullOrWhiteSpace(searchBox.Text) ? Visibility.Visible : Visibility.Collapsed;

            var query = searchBox.Text.ToLower();
            var filtered = allFileBoxes.Where(f => f.FileName.Replace(".pdf", "").Contains(query)).ToList();

            Log.Information($"Suche wurde gestartet mit dem Query={query}.");

            RenderBoxes(filtered);
        }

        private void UploadBox_Click(object sender, MouseButtonEventArgs e)
        {
            Log.Information("UploadBox wurde geklickt.");

            var dialog = new OpenFileDialog
            {
                Filter = "PDF-Dateien (*.pdf)|*.pdf",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                AddFileBoxFromPath(dialog.FileName);
            }
        }

        private void UploadBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void UploadBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    string extension = System.IO.Path.GetExtension(files[0]).ToLower();
                    Log.Information($"Empfangene Datei={files[0]}.");

                    if (extension == ".pdf")
                    {
                        AddFileBoxFromPath(files[0]);
                    }
                    else
                    {
                        Log.Warning("Ungültiger Dateityp.");
                        MessageBox.Show("Fehler nur pdfs erlaubt!");
                    }
                }
            }
        }

        private void AddFileBoxFromPath(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLower();

            if (extension != ".pdf")
            {
                Log.Warning("Ungültiger Dateityp.");
                MessageBox.Show("Fehler nur pdfs erlaubt!");
                return;
            }

            ImageBrush previewBrush = null;

            try
            {
                using (var pdfDoc = PdfiumViewer.PdfDocument.Load(filePath))
                using (var image = pdfDoc.Render(0, 300, 400, true))
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    previewBrush = new ImageBrush(bitmapImage)
                    {
                        Stretch = Stretch.UniformToFill
                    };

                    Log.Information("FilePreviewWindow wurde erfolgreich generiert.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler beim laden oder rendern der Datei.");
                MessageBox.Show(ex.Message);
                return;
            }

            string boxType = "file";
            var fileBox = new FileBoxControl("file", System.IO.Path.GetFileName(filePath), filePath);
            fileBox.ToolTip = System.IO.Path.GetFileName(filePath);

            if (previewBrush != null)
            {
                fileBox.SetBackground(previewBrush);
            }

            allFileBoxes.Add(fileBox);
            fileBoxPanel.Children.Add(fileBox);

            Log.Information($"FileBox wurde hinzugefügt die den Pfad={filePath} hat.");
        }

        private void RenderBoxes(IEnumerable<FileBoxControl> boxes)
        {
            fileBoxPanel.Children.Clear();

            foreach (var box in boxes)
            {
                fileBoxPanel.Children.Add(box);
            }
        }
    }
}