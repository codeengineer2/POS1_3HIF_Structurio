using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurio.Classes
{
    public class Column
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BoardId { get; set; }
        public List<Issue> Issues { get; set; }
    }
}