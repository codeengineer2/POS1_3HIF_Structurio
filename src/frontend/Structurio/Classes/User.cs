using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    public class User
    {
        [JsonProperty("uid")]
        public int Id { get; set; }

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("birthdate")]
        public string Birthdate { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}