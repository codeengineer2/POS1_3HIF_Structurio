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
using Structurio.Classes;
using Structurio.Controls;

namespace Structurio.Pages
{
    /// <summary>
    /// Interaction logic for KanbanPage.xaml
    /// </summary>
    public partial class KanbanPage : Page
    {
        private Project project;
        public ObservableCollection<ColumnWrapper> Columns { get; set; } = new();
        private int issueCounter = 1;

        public KanbanPage(Project project)
        {
            InitializeComponent();
            this.project = project;

            foreach (var column in project.Board.Columns)
            {
                Columns.Add(new ColumnWrapper(column));
            }

            kanbanItemsControl.ItemsSource = Columns;
        }

        private void addItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addColumn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void titleBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}