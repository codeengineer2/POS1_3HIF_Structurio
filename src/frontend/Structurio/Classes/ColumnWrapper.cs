using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Structurio.Classes
{
    public class ColumnWrapper
    {
        public Column Original { get; }
        public string Name
        {
            get
            {
                return this.Original.Name;
            }
            set
            {
                this.Original.Name = value;
            }
        }
        public ObservableCollection<Issue> Items { get; }

        public ColumnWrapper(Column column)
        {
            this.Original = column;
            this.Items = new ObservableCollection<Issue>(column.Issues ?? new List<Issue>());
        }
    }
}