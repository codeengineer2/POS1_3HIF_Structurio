using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    public class AddIssueRequest
    {
        [JsonProperty("column_id")]
        public int ColumnId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}