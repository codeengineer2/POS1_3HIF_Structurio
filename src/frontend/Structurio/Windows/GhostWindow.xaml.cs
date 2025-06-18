using Serilog;
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
    /// Fenster für Drag Vorschau eines Elements.
    /// </summary>
    public partial class GhostWindow : Window
    {
        /// <summary>
        /// Zeigt das übergebene UI Element im GhostWindow an.
        /// </summary>
        /// <param name="visual">Das Element zur Vorschau.</param>
        public GhostWindow(UIElement visual)
        {
            InitializeComponent();

            if (visual != null)
            {
                contentPresenter.Content = visual;
                Log.Debug("GhostWindow wurde erfolgreich geladen.");
            }
            else
            {
                Log.Warning("GhostWindow wurde nicht korrekt geladen.");
            }
        }
    }
}