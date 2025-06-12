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

namespace Structurio.Windows
{
    /// <summary>
    /// Interaction logic for AddIssueWindow.xaml
    /// </summary>
    public partial class AddIssueWindow : Window
    {
        public string IssueDescription { get; private set; }

        public AddIssueWindow()
        {
            InitializeComponent();
        }

        private void descriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            descriptionBox.ClearValue(BackgroundProperty);
            descriptionInfo.Text = "* erforderlich";
            descriptionInfo.Foreground = Brushes.Gray;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            IssueDescription = descriptionBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(IssueDescription))
            {
                descriptionBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                descriptionInfo.Text = "Bitte Beschreibung eingeben!";
                descriptionInfo.Foreground = Brushes.DarkRed;
                return;
            }

            if (IssueDescription.Length > 100)
            {
                descriptionBox.Background = new SolidColorBrush(Color.FromRgb(255, 235, 235));
                descriptionInfo.Text = "Max. 100 Zeichen erlaubt!";
                descriptionInfo.Foreground = Brushes.DarkRed;
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}