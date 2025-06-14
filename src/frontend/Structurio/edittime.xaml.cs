using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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

namespace Structurio
{
    /// <summary>
    /// Interaction logic for edittime.xaml
    /// </summary>
    public partial class edittime : Window
    {
        public int Index;
        DataGrid Times;
        private readonly ObservableCollection<Timecheckin> entries;
        private readonly HttpClient httpClient;
        private readonly int uid = 1;
        public edittime(ObservableCollection<Timecheckin> entry, int index, DataGrid times, HttpClient httpClient)
        {
            InitializeComponent();
            Index = index;
            entries = entry;
            DataContext = this;
            datein.SelectedDate = entries[index].CheckIN.Date;
            dateout.SelectedDate = entries[index].CheckOUT.Date;

            hourin.SelectedIndex = entries[index].CheckIN.Hour - 1;
            

            hourout.SelectedIndex = entries[index].CheckOUT.Hour - 1;
      
            Times =times;
            this.httpClient = httpClient;

        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!datein.SelectedDate.HasValue || !dateout.SelectedDate.HasValue)
                {
                    MessageBox.Show("Bitte für Ein- und Ausstempel-Datum ein Datum wählen.");
                    return;
                }

                if (hourin.SelectedItem == null || hourout.SelectedItem == null)
                {
                    MessageBox.Show("Bitte mindestens die Stunden für Ein- und Ausstempel-Zeiten auswählen.");
                    return;
                }

                int hourIn = int.Parse((hourin.SelectedItem as ComboBoxItem).Content.ToString());
                int hourOut = int.Parse((hourout.SelectedItem as ComboBoxItem).Content.ToString());

                int minIn, minOut;
                if (minin.SelectedItem != null)
                    minIn = int.Parse((minin.SelectedItem as ComboBoxItem).Content.ToString());
                else
                    minIn = entries[Index].CheckIN.Minute;

                if (minout.SelectedItem != null)
                    minOut = int.Parse((minout.SelectedItem as ComboBoxItem).Content.ToString());
                else
                    minOut = entries[Index].CheckOUT.Minute;

                DateTime checkIn = datein.SelectedDate.Value.Date + new TimeSpan(hourIn, minIn, 0);
                DateTime checkOut = dateout.SelectedDate.Value.Date + new TimeSpan(hourOut, minOut, 0);

                if (checkOut < checkIn)
                {
                    MessageBox.Show("CheckOUT darf nicht vor CheckIN liegen.");
                    return;
                }

                entries[Index].CheckIN = checkIn;
                entries[Index].CheckOUT = checkOut;
                TimeSpan duration = checkOut - checkIn;
                int totalHours = (int)duration.TotalHours;
                int minutes = duration.Minutes;
                entries[Index].Duration = $"{totalHours:D2}:{minutes:D2}";

                Times.Items.Refresh();
                await Put_timestamp.UpdateAsync(
                    httpClient,
                    uid,
                    entries[Index].Zid,
                    checkIn,
                    checkOut,
                    $"{totalHours:D2}:{minutes:D2}");

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern der Zeitdaten: " + ex.Message);
            }

        }


    }
}
