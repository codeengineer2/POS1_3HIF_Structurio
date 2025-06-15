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
            uid = user.Id;
            times.ItemsSource = entries;
            LoadTimestamps();
        }
        public async void LoadTimestamps()
        {
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
                times.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Zeitstempel: {ex.Message}");
            }
        }
        

        private async void Checking(object sender, RoutedEventArgs e)
        {
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
                    times.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Check-in: {ex.Message}");
                }
            }


        }

        private async void Checkout(object sender, RoutedEventArgs e)
        {
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Check-out: {ex.Message}");
                }
            }



        }

        private void Dataändern(object sender, RoutedEventArgs e)
        {
            if(times.SelectedValue is not null)
            {
                Window aender = new edittime(entries, timeindex, times, httpClient);

                aender.ShowDialog();
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
