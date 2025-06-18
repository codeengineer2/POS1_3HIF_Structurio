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
using Structurio.Classes;
using Serilog;

namespace Structurio
{
    /// <summary>
    /// Interaction logic for Costs.xaml
    /// </summary>
    public partial class Costs : Page
    {

        public ObservableCollection<Finance> finance = new ObservableCollection<Finance>();
        private string rechnungspfad;
        private readonly HttpClient httpClient;
        private int uid = 1;
        private int pid = 1;
        private double gesamtBudget = 10000;
        public Costs(User user, Project project)
        {
            InitializeComponent();
            Log.Information("Costs.xaml: Window initialisiert");
            uid = user.Id;
            pid = project.Id;
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:8080/")
            };
            
            CostsListView.ItemsSource = finance;
            
            Load_Items();
          
        }
        private async void Load_Items()
        {
            Log.Information("Costs.xaml: Lade die Abrechnungen für User={UserId} und Project={ProjectId}", uid, pid);
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
                UpdatePieChart();
                Log.Information("Costs.xaml: Die Abrechnungen wurden erfolgreich geladen, Anzahl Einträge: {Count}", finance.Count);
            }
            
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Zeitstempel: {ex.Message}");
            }
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Costs.xaml: Upload Klick");
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
                    Log.Information("Costs.xaml: Datei erfolgreich hochgeladen: {FilePath}", rechnungspfad);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Hochladen:\n" + ex.ToString(), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log.Error(ex, "Costs.xaml: Fehler beim Hochladen der Datei: {FilePath}", openFileDialog.FileName);  
                }
            }
        }

        private async void Save_Data_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Costs: Save_Data Klick");
            if (string.IsNullOrWhiteSpace(costsName.Text) || string.IsNullOrWhiteSpace(costs.Text) || !DatePickerCosts.SelectedDate.HasValue || CostsCategory.SelectedItem == null || string.IsNullOrWhiteSpace(rechnungspfad))
            {
                MessageBox.Show("Bitte füllern sie alle Felder aus!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("Costs.xaml: Speichern fehlgeschlagen, eines oder mehrere Felder sind leer.");
                return;
            }
            
            
            if (!double.TryParse(costs.Text, out double preis))
            {
                MessageBox.Show("Bitte geben Sie einen gültigen Preis ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Warning("Costs.xaml: Speichern fehlgeschlagen, ungültiger Preis eingegeben: {Preis}", costs.Text);
                return;
            }
            string name = costsName.Text;
            DateTime datum = DatePickerCosts.SelectedDate.Value;
            string kategorie = ((ComboBoxItem)CostsCategory.SelectedItem).Content.ToString();

            Abrechnung_JSON json;
            try
            {
                json = await Post_Abrechnung.CreateAsync(
                                httpClient,
                                uid,                    
                                pid,
                                costsName.Text,
                                DatePickerCosts.SelectedDate.Value,
                                preis,
                                kategorie,
                                rechnungspfad);
                Log.Information("Costs.xaml: Abrechnung Id={aid} gespeichert", json.Aid);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Speichern fehlgeschlagen:\n{ex.Message}",
                                "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "Costs.xaml: Fehler beim Speichern der Abrechnung");
                return;
            }
            finance.Add(new Finance
            {
                Name = name,
                Preis = preis,
                Datum = datum,
                Kategorie = kategorie,
                Rechnung = rechnungspfad
            });
            UpdatePieChart();
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
        private void CostsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Log.Information("Costs.xaml: Kosten Eintrag doppelt angeklickt");
            if (CostsListView.SelectedItem is Finance fin)
            {
                var detailWindow = new Costs_Detail(fin);
                detailWindow.Show();
            }
        }
        private void UpdatePieChart()
        {
            Log.Information("Costs.xaml: Updaten des Chartes gestartet");

            var kategorien = new[] { "Lizenzen und Abos", "Meetings", "Hardware", "Arbeitsmittel" };
            var kategorienSummen = finance
                .GroupBy(f => f.Kategorie)
                .ToDictionary(g => g.Key, g => g.Sum(item => item.Preis));

            var seriesList = new List<PieSeries<double>>();
            double gesamtausgaben = 0;

            foreach (var kategorie in kategorien)
            {
                double betrag = kategorienSummen.ContainsKey(kategorie) ? kategorienSummen[kategorie] : 0;
                gesamtausgaben += betrag;

                if (betrag > 0)
                {
                    seriesList.Add(new PieSeries<double>
                    {
                        Values = new[] { betrag },
                        Name = kategorie,
                        DataLabelsSize = 14,
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                        DataLabelsFormatter = point => $"{point.Label}: {point.Model} €"
                    });
                    Log.Information("Costs.xaml: Kategorie {Kategorie} mit Betrag {Betrag} hinzugefügt", kategorie, betrag);
                }
            }

            double restBudget = gesamtBudget - gesamtausgaben;
            if (restBudget > 0)
                seriesList.Add(new PieSeries<double>
                {
                    Values = new[] { restBudget },
                    Name = "Verfügbar",
                    DataLabelsSize = 14,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => $"{point.Label}: {point.Model} €"
                });

            if (gesamtausgaben == 0)
            {
                seriesList.Clear();
                seriesList.Add(new PieSeries<double>
                {
                    Values = new[] { gesamtBudget },
                    Name = "Verfügbar",
                    DataLabelsSize = 14,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => $"{point.Label}: {point.Model} €"
                });
            }

            PieChartCosts.Series = seriesList;
            Log.Information("Costs.xaml: Chart erfolgreich aktualisiert; Gesamtausgaben: {Gesamtausgaben}, Verfügbares Budget: {VerfügbaresBudget}", gesamtausgaben, restBudget);
        }
    }
}
