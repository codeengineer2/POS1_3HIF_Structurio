using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    /// <summary>
    /// Anfrage um neues Issue zu machen
    /// </summary>
    public class AddIssueRequest
    {
        /// <summary>
        /// ID von der Spalte
        /// </summary>
        [JsonProperty("column_id")]
        public int ColumnId { get; set; }

        /// <summary>
        /// Text vom Issue
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}