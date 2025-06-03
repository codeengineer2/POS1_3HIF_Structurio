using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

namespace Structurio
{
    /// <summary>
    /// Interaction logic for edittime.xaml
    /// </summary>
    public partial class edittime : Window
    {
        public int Index;
        ListView Times;
        public ObservableCollection<Timecheckin> Entries { get; set; }

        public edittime(ObservableCollection<Timecheckin> entries, int index, ListView times)
        {
            InitializeComponent();
            Index = index;
            Entries = entries;
            DataContext = this;
            datein.SelectedDate = Entries[index].CheckIN.Date;
            dateout.SelectedDate = Entries[index].CheckOUT.Date;

            hourin.SelectedIndex = Entries[index].CheckIN.Hour - 1;
            

            hourout.SelectedIndex = Entries[index].CheckOUT.Hour - 1;
      
            Times =times;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
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
                    minIn = Entries[Index].CheckIN.Minute;

                if (minout.SelectedItem != null)
                    minOut = int.Parse((minout.SelectedItem as ComboBoxItem).Content.ToString());
                else
                    minOut = Entries[Index].CheckOUT.Minute;

                DateTime checkIn = datein.SelectedDate.Value.Date + new TimeSpan(hourIn, minIn, 0);
                DateTime checkOut = dateout.SelectedDate.Value.Date + new TimeSpan(hourOut, minOut, 0);

                if (checkOut < checkIn)
                {
                    MessageBox.Show("CheckOUT darf nicht vor CheckIN liegen.");
                    return;
                }

                Entries[Index].CheckIN = checkIn;
                Entries[Index].CheckOUT = checkOut;
                TimeSpan duration = checkOut - checkIn;
                int totalHours = (int)duration.TotalHours;
                int minutes = duration.Minutes;
                Entries[Index].Duration = $"{totalHours:D2}:{minutes:D2}";

                Times.Items.Refresh();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern der Zeitdaten: " + ex.Message);
            }

        }


    }
}
