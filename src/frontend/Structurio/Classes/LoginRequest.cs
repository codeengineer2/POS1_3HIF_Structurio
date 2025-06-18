using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Structurio.Classes
{
    /// <summary>
    /// Anfrage mit Email und Passwort
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// EMail vom Benutzer
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Passwort vom Benutzer
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}