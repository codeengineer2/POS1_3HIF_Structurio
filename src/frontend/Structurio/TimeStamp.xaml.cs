using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Structurio
{
    /// <summary>
    /// Interaction logic for TimeStamp.xaml
    /// </summary>
    public partial class TimeStamp : Page
    {
        public ObservableCollection<Timecheckin> entries = new ObservableCollection<Timecheckin>();
        public int timeindex = 0;
        public TimeStamp()
        {
            InitializeComponent();
            times.ItemsSource = entries;
        }
        

        private void Checking(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;

            


            if (entries.Count== 0 || entries[entries.Count - 1].CheckOUT != DateTime.MinValue)
            {
                entries.Add(new Timecheckin
                {
                    CheckIN = now,
                    CheckOUT = DateTime.MinValue,
                    Duration = "0:00"

                }) ;  

            }
           
            
        }

        private void Checkout(object sender, RoutedEventArgs e)
        {
            if (entries.Count > 0 && entries[entries.Count-1].CheckOUT == DateTime.MinValue)
            {
                DateTime now = DateTime.Now;

                entries[entries.Count-1].CheckOUT = now;
                entries[entries.Count-1].Duration = (now - entries[entries.Count - 1].CheckIN).ToString(@"hh\:mm");
                // List Refresh auslösen
                times.Items.Refresh();
            }



        }

        private void Dataändern(object sender, RoutedEventArgs e)
        {
            if(times.SelectedValue is not null)
            {
                Window aender = new edittime(entries, timeindex, times);

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

        private void ButtonCosts_Click(object sender, RoutedEventArgs e)
        {
            Costs costsWindow = new Costs();
            costsWindow.Show();
        }
    }
}
