using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    /// <summary>
    /// Projekt mit Name, Farbe, Beschreibung und Board
    /// </summary>
    public class Project
    {
        /// <summary>
        /// ID vom Projekt
        /// </summary>
        [JsonProperty("pid")]
        public int Id { get; set; }

        /// <summary>
        /// Name vom Projekt
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Beschreibung
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Farbe als String (z. B. Hex)
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Das zugehörige Board
        /// </summary>
        public Board Board { get; set; }

        /// <summary>
        /// User-ID vom Besitzer
        /// </summary>
        public int OwnerUid { get; set; }
    }
}