using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    /// <summary>
    /// Anfrage um Spaltennamen zu ändern
    /// </summary>
    public class UpdateColumnRequest
    {
        /// <summary>
        /// ID von der Spalte
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Neuer Name der Spalte
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}