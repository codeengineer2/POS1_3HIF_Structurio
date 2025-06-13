using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Win32;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Printing;
using System.Security.Cryptography;
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

        public ObservableCollection<Finance> finance = new ObservableCollection<Finance>();
        private string rechnungspfad;
        private readonly HttpClient httpClient;
        private readonly int uid = 1;
        private readonly int pid = 1;
        public Costs()
        {
            InitializeComponent();
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080/")
            };
            PieChartCosts.Series = [

                new PieSeries<double> { Values = new double[] { 1000 }, Name="Lohn" },
                new PieSeries<double> { Values = new double[] { 8000 }, Name="Lizenzen" },
                new PieSeries<double> { Values = new double[] { 2000 }, Name="Essen" },
            ];
            CostsListView.ItemsSource = finance;
           
            Load_Items();
        }
        private async void Load_Items()
        {
            try
            {
                var items = await Get_Abrechnung.GetAsync(httpClient, uid, pid);
                Debug.WriteLine($"Zurückgelieferte Einträge: {items.Count}");
                finance.Clear();
                foreach (var item in items)
                {
                    finance.Add(new Finance
                    {
                        Name = item.Name,
                        Preis = item.Price,
                        Datum = item.Date,
                        Kategorie = item.Category,
                        Rechnung = item.Rechnung
                    });
                }
                CostsListView.Items.Refresh();
            }
            
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Zeitstempel: {ex.Message}");
            }
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
                string relativePathWithFileName = IOPath.Combine(relativePath, dateiName);

                try
                {
                    File.Copy(openFileDialog.FileName, zielPfad, overwrite: true);
                    rechnungspfad = relativePathWithFileName;
                    MessageBox.Show("Datei erfolgreich hochgeladen!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Hochladen:\n" + ex.ToString(), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Save_Data_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(costsName.Text) || string.IsNullOrWhiteSpace(costs.Text) || !DatePickerCosts.SelectedDate.HasValue || CostsCategory.SelectedItem == null || string.IsNullOrWhiteSpace(rechnungspfad))
            {
                MessageBox.Show("Bitte füllern sie alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            
            if (!double.TryParse(costs.Text, out double preis))
            {
                MessageBox.Show("Bitte geben Sie einen gültigen Preis ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show(preis.ToString());

            string name = costsName.Text;
            DateTime datum = DatePickerCosts.SelectedDate.Value;
            string kategorie = ((ComboBoxItem)CostsCategory.SelectedItem).Content.ToString();

            finance.Add(new Finance
            {
                Name = name,
                Preis = preis,
                Datum = datum,
                Kategorie = kategorie,
                Rechnung = rechnungspfad
            });
            CostsListView.Items.Refresh();
            CostsCategory.SelectedIndex = -1;
            costsName.Text = "";
            costs.Text = "";
            DatePickerCosts.SelectedDate = null;
            rechnungspfad = "";
        }
        private void Window_SizeChanged(object Sender, SizeChangedEventArgs e)
        {
            double totalWidth = CostsListView.ActualWidth - 35;

            if (CostsListView.View is GridView gridView && gridView.Columns.Count == 5)
            {
                gridView.Columns[0].Width = totalWidth * 0.20;
                gridView.Columns[1].Width = totalWidth * 0.10;
                gridView.Columns[2].Width = totalWidth * 0.15;
                gridView.Columns[3].Width = totalWidth * 0.25;
                gridView.Columns[4].Width = totalWidth * 0.30;
            }
        }
    }
}
