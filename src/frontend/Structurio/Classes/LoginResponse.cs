using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structurio.Classes
{
    /// <summary>
    /// Antwort nach Login mit User und Projekten
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// true wenn Login ok
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Der eingeloggte Benutzer
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Projekte vom Benutzer
        /// </summary>
        public List<Project> Projects { get; set; }
    }
}