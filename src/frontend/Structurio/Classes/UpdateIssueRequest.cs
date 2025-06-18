using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    /// <summary>
    /// Anfrage um Issue zu ändern
    /// </summary>
    public class UpdateIssueRequest
    {
        /// <summary>
        /// ID vom Issue
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Neue Beschreibung
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}