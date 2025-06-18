using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    /// <summary>
    /// Anfrage um neue Spalte zu machen
    /// </summary>
    public class AddColumnRequest
    {
        /// <summary>
        /// ID vom Board
        /// </summary>
        [JsonProperty("board_id")]
        public int BoardId { get; set; }

        /// <summary>
        /// Name der Spalte
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}