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

        private void UploadBox_Click(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PDF-Dateien|*.pdf|Bilder|*.png;*.jpg;*.jpeg",
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
                    AddFileBoxFromPath(files[0]);
                }
            }
        }

        private void AddFileBoxFromPath(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLower();
            ImageBrush previewBrush = null;

            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                var image = new BitmapImage(new Uri(filePath));
                previewBrush = new ImageBrush(image) { Stretch = Stretch.UniformToFill };
            }
            else if (extension == ".pdf") // using a package :/
            {
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
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            string boxType = filePath.ToLower().Contains("diagram") || extension == ".pdf" ? "diagram" : "file";
            var fileBox = new FileBoxControl(boxType, System.IO.Path.GetFileName(filePath));

            fileBox.ToolTip = System.IO.Path.GetFileName(filePath);

            if (previewBrush != null)
            {
                fileBox.SetBackground(previewBrush);
            }

            fileBoxPanel.Children.Add(fileBox);
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