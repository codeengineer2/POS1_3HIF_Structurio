using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    public class AddColumnRequest
    {
        [JsonProperty("board_id")]
        public int BoardId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
