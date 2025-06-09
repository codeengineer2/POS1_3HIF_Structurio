using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurio.Classes
{
    public class Board
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public List<Column> Columns { get; set; }
    }
}