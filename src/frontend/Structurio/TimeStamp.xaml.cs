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
        ObservableCollection<Timecheckin> entries = new ObservableCollection<Timecheckin>();
        public TimeStamp()
        {
            InitializeComponent();
            times.ItemsSource = entries;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;

            DateOnly dateOnly = DateOnly.FromDateTime(now);
            TimeOnly timeOnly = TimeOnly.FromDateTime(now);

           if (entries.Count== 0 || entries[entries.Count - 1].CheckOUT != null)
            {
                entries.Add(new Timecheckin
                {
                    Date = dateOnly,
                    CheckIN = timeOnly,
                    CheckOUT = TimeOnly.MinValue,
                    Duration = "0:00"

                }) ;
            }
           
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (entries.Count > 0 && entries[entries.Count-1].CheckOUT == TimeOnly.MinValue)
            {
                DateTime now = DateTime.Now;

                DateOnly dateOnly = DateOnly.FromDateTime(now);
                TimeOnly timeOnly = TimeOnly.FromDateTime(now);

                entries[entries.Count-1].CheckOUT = timeOnly;


                // List Refresh auslösen
                times.Items.Refresh();
            }



        }
    }
}
