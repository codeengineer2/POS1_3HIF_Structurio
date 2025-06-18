using Microsoft.Web.WebView2.Core;
using PdfiumViewer;
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
using IOPath = System.IO.Path;
using System.IO;
using Serilog;

/// @file costs_Detail.xaml.cs
/// @brief Detailansicht für einzelne Kosten.


namespace Structurio
{
    /// <summary>
    /// Interaktionslogik für Costs_Detail.xaml
    /// </summary>
    /// @class Costs_Detail
    /// @brief Fenster zum Anzeigen eines einzelnen Abrechnungseintrags mit PDF-Vorschau.
    public partial class Costs_Detail : Window
    {

        private readonly string relativePfad;
        private readonly string name, kategorie;
        private readonly double kosten;
        private readonly DateTime datum;

        /// @brief Konstruktor, initialisiert GUI mit Abrechnungsdaten.
        /// @param finance Abrechnungsobjekt mit Name, Preis, Kategorie usw.
        /// @return Kein Rückgabewert.
        public Costs_Detail(Finance finance)
        {
            InitializeComponent();

            relativePfad = finance.Rechnung;
            name = finance.Name;
            kosten = finance.Preis;
            datum = finance.Datum;
            kategorie = finance.Kategorie;

            LabelName.Text = name;
            LabelKosten.Text = $"{kosten:F2} €";
            LabelDatum.Text = datum.ToShortDateString();
            LabelKategorie.Text = kategorie;
            LabelPfad.Text = relativePfad;

            PdfView.CoreWebView2InitializationCompleted += WebViewReady;
            _ = PdfView.EnsureCoreWebView2Async();
            Log.Information("Costs_Detail.xaml: Window initialisiert");
        }
        /// @brief Initialisiert die PDF-Vorschau mit WebView2 oder zeigt Fehler.
        /// @param sender Event-Quelle.
        /// @param e Enthält Erfolg oder Fehlerstatus der Initialisierung.
        /// @return Kein Rückgabewert.
        private void WebViewReady(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                ErrorText.Text = $"PDF-Viewer konnte nicht gestartet werden";
                ErrorText.Visibility = Visibility.Visible;
                Log.Error("WebView2 konnte nicht gestartet werden");
                return;
            }

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = IOPath.GetFullPath(IOPath.Combine(baseDir, relativePfad));

            if (!File.Exists(fullPath))
            {
                ErrorText.Text = "PDF nicht gefunden.";
                ErrorText.Visibility = Visibility.Visible;
                Log.Error("Kosten-Dokument nicht gefunden: {Path}", fullPath);
                return;
            }

            PdfView.Visibility = Visibility.Visible;
            PdfView.CoreWebView2.Navigate(new Uri(fullPath).AbsoluteUri);
            Log.Information("Kosten-Dokument geladen: {Path}", fullPath);
        }


    }

}
