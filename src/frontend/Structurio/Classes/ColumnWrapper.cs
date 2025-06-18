using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Structurio.Classes
{
    /// <summary>
    /// Wrapper für Column mit Observable Items
    /// </summary>
    public class ColumnWrapper
    {
        /// <summary>
        /// Originale Column
        /// </summary>
        public Column Original { get; }

        /// <summary>
        /// Name der Spalte
        /// </summary>
        public string Name
        {
            get { return this.Original.Name; }
            set { this.Original.Name = value; }
        }

        /// <summary>
        /// Observable Liste mit Issues
        /// </summary>
        public ObservableCollection<Issue> Items { get; }

        /// <summary>
        /// Baut Wrapper mit Column
        /// </summary>
        public ColumnWrapper(Column column)
        {
            this.Original = column;
            this.Items = new ObservableCollection<Issue>(column.Issues ?? new List<Issue>());
        }
    }
}