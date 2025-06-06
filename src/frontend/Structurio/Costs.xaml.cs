using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Structurio
{
    /// <summary>
    /// Interaction logic for Costs.xaml
    /// </summary>
    public partial class Costs : Window
    {
        public Costs()
        {
            InitializeComponent();

            PieChartCosts.Series = [
            
                new PieSeries<double> { Values = new double[] { 1000 }, Name="Lohn" },
                new PieSeries<double> { Values = new double[] { 8000 }, Name="Lizenzen" },
                new PieSeries<double> { Values = new double[] { 2000 }, Name="Essen" },
            ];

        }
    }
}
