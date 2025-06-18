using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using Structurio.Classes;
using Serilog;

namespace Structurio
{
    /// <summary>
    /// Interaction logic for TimeStamp.xaml
    /// </summary>
    public partial class TimeStamp : Page
    {
        private readonly HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };
        public ObservableCollection<Timecheckin> entries = new ObservableCollection<Timecheckin>();
        private int uid = 1;
        private int pid = 1;
        public int timeindex = 0;
     
        public TimeStamp(User user)
        {
            InitializeComponent();
            Log.Information("TimeStamp.xaml: Window initialisiert");
            uid = user.Id;
            times.ItemsSource = entries;
            LoadTimestamps();
        }
        public async void LoadTimestamps()
        {
            Log.Information("TimeStamp: Lädt Timestamps");

            try
            {
                var items = await Get_timestamp.GetAsync(httpClient, uid);
                entries.Clear();
                foreach (var item in items)
                {
                    DateTime checkIn = DateTime.Parse($"{item.datum_in} {item.checkin}");
                    DateTime checkOut = DateTime.Parse($"{item.datum_out} {item.checkout}");
                    entries.Add(new Timecheckin
                    {
                        Zid = item.zid,
                        CheckIN = checkIn,
                        CheckOUT = checkOut,
                        Duration = item.duration
                    });
                }
                Log.Information("TimeStamp: Timestamps geladen");
                times.Items.Refresh();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "TimeStamp: Es gab Fehler beim Laden der Timestamps");
               
            }
        }
        

        private async void Checking(object sender, RoutedEventArgs e)
        {
            Log.Information("TimeStamp: CheckIn Klick");
            DateTime now = DateTime.Now;
            bool needsNew = entries.Count == 0 || entries.Last().CheckOUT != DateTime.MinValue;
            if (needsNew)
            {
                try
                {
                    var created = await Post_timestamp.CreateAsync(httpClient, uid, now);
                    entries.Add(new Timecheckin
                    {
                        Zid = created.zid,
                        CheckIN = now,
                        CheckOUT = DateTime.MinValue,
                        Duration = "00:00"
                    });
                    Log.Information("TimeStamp: CheckIn Erfolgreich");
                    times.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Check-in: {ex.Message}");
                    Log.Information("TimeStamp: CheckIn fehlgeschlagen");
                }
            }


        }

        private async void Checkout(object sender, RoutedEventArgs e)
        {
            Log.Information("TimeStamp: CheckOut Klick");
            var entry = entries.LastOrDefault();
            if (entry != null && entry.CheckOUT == DateTime.MinValue)
            {
                DateTime now = DateTime.Now;
                entry.CheckOUT = now;
                entry.Duration = (now - entry.CheckIN).ToString(@"hh\:mm");
                times.Items.Refresh();

                try
                {
                    await Put_timestamp.UpdateAsync(
                        httpClient,
                        uid,
                        entry.Zid,
                        entry.CheckIN,
                        entry.CheckOUT,
                        entry.Duration);
                    Log.Information("TimeStamp: CheckOut Erfolgreich");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Check-out: {ex.Message}");
                    Log.Error(ex, "TimeStamp: CheckOut fehlgeschlagen");
                }
            }



        }

        private void Dataaendern(object sender, RoutedEventArgs e)
        {
            Log.Information("TimeStamp: Aendern Klick");
            if (times.SelectedValue is not null)
            {
                Window aender = new edittime(uid, entries, timeindex, times, httpClient);

                aender.ShowDialog();
                Log.Information("TimeStamp: Aendern Fenster geöffnet");
            }
            
        }
            
        private void changed_clicktime(object sender, SelectionChangedEventArgs e)
        {
            if (times.SelectedIndex <= entries.Count)
            {
                timeindex = times.SelectedIndex;
            }
        }

       
    }
}
