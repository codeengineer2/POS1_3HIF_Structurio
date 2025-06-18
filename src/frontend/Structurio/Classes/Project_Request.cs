using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    /// <summary>
    /// Anfrage um neues Projekt zu erstellen
    /// </summary>
    public class ProjectRequest
    {
        /// <summary>
        /// Name vom Projekt
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Beschreibung vom Projekt
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Farbe vom Projekt
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; }

        /// <summary>
        /// User-ID vom Besitzer
        /// </summary>
        [JsonProperty("owner_uid")]
        public int OwnerUid { get; set; }
    }
}