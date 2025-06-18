using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Structurio.Classes
{
    /// <summary>
    /// Macht aus Farbe im Text eine Brush
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Wandelt Hex-String in Brush um
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hex)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
            }
            return Brushes.DarkGray;
        }

        /// <summary>
        /// Zurückwandeln nicht gemacht
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}