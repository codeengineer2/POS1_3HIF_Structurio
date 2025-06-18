using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    /// <summary>
    /// User mit Name, Geburtsdatum und EMail
    /// </summary>
    public class User
    {
        /// <summary>
        /// User-ID
        /// </summary>
        [JsonProperty("uid")]
        public int Id { get; set; }

        /// <summary>
        /// Vorname
        /// </summary>
        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        /// <summary>
        /// Nachname
        /// </summary>
        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        /// <summary>
        /// Geburtsdatum als Text
        /// </summary>
        [JsonProperty("birthdate")]
        public string Birthdate { get; set; }

        /// <summary>
        /// EMail-Adresse
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}