using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    public class Project
    {
        [JsonProperty("pid")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public Board Board { get; set; }
        public int OwnerUid { get; set; }
    }
}   