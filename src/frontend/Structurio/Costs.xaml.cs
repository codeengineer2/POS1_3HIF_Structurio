using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Win32;
using SkiaSharp;
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
using IOPath = System.IO.Path;

namespace Structurio
{
    /// <summary>
    /// Interaction logic for Costs.xaml
    /// </summary>
    public partial class Costs : Window
    {
        public Costs()
        {
            InitializeComponent();

            PieChartCosts.Series = [

                new PieSeries<double> { Values = new double[] { 1000 }, Name="Lohn" },
                new PieSeries<double> { Values = new double[] { 8000 }, Name="Lizenzen" },
                new PieSeries<double> { Values = new double[] { 2000 }, Name="Essen" },
            ];

        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = @"..\..\..\Rechnung\";

                string zielOrdner = IOPath.GetFullPath(IOPath.Combine(baseDir, relativePath));

         

                if (!Directory.Exists(zielOrdner))
                {
                    Directory.CreateDirectory(zielOrdner);
                }

                string dateiName = IOPath.GetFileName(openFileDialog.FileName);
                string zielPfad = IOPath.Combine(zielOrdner, dateiName);

                try
                {
                    File.Copy(openFileDialog.FileName, zielPfad, overwrite: true);
                    MessageBox.Show("Datei erfolgreich hochgeladen!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Hochladen:\n" + ex.ToString(), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
