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
            MessageBox.Show(entries[index] + "liste, index: " + index);
            timein.Text = Entries[index].CheckIN.ToString("hh:mm tt");
            timeout.Text = Entries[index].CheckOUT.ToString("hh:mm tt");
            datein.DisplayDate = Entries[index].CheckIN;
            dateout.DisplayDate = Entries[index].CheckOUT;
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

                var inTime = DateTime.ParseExact(timein.Text, "hh:mm tt", CultureInfo.InvariantCulture);
                var outTime = DateTime.ParseExact(timeout.Text, "hh:mm tt", CultureInfo.InvariantCulture);

                Entries[Index].CheckIN = datein.SelectedDate.Value.Date + inTime.TimeOfDay;
                Entries[Index].CheckOUT = dateout.SelectedDate.Value.Date + outTime.TimeOfDay;

                Entries[Index].Duration = (Entries[Index].CheckOUT - Entries[Index].CheckIN)
                                          .ToString(@"hh\:mm");

                Times.Items.Refresh();
                Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Uhrzeit bitte im Format 01:02 PM eingeben.");
            }

        }


    }
}
