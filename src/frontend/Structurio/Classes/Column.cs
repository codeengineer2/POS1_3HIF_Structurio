using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurio.Classes
{
    /// <summary>
    /// Spalte gehört zu Board hat Issues
    /// </summary>
    public class Column
    {
        /// <summary>
        /// ID von der Spalte
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name der Spalte
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ID vom Board
        /// </summary>
        public int BoardId { get; set; }

        /// <summary>
        /// Liste mit Issues
        /// </summary>
        public List<Issue> Issues { get; set; }
    }
}