using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurio.Classes
{
    /// <summary>
    /// Board gehört zu Projekt hat Spalten
    /// </summary>
    public class Board
    {
        /// <summary>
        /// ID vom Board
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID vom Projekt
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Liste mit Spalten
        /// </summary>
        public List<Column> Columns { get; set; }
    }
}