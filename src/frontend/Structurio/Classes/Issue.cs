using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurio.Classes
{
    /// <summary>
    /// Issue gehört zu einer Spalte und hat Beschreibung
    /// </summary>
    public class Issue : INotifyPropertyChanged
    {
        /// <summary>
        /// ID vom Issue
        /// </summary>
        public int Id { get; set; }

        private string description;

        /// <summary>
        /// Text vom Issue
        /// </summary>
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        /// <summary>
        /// ID von der Spalte
        /// </summary>
        public int ColumnId { get; set; }

        /// <summary>
        /// Optionaler Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Event für PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Wird aufgerufen wenn sich Property ändert
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}