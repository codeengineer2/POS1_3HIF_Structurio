using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurio.Classes;

namespace Structurio.Interfaces
{
    /// <summary>
    /// Authentifizierung für Login, Register, Logout
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Versucht Login mit EMail und Passwort
        /// </summary>
        Task<bool> TryLogin(string email, string password);

        /// <summary>
        /// Registriert neuen User
        /// </summary>
        Task<bool> Register(RegisterRequest request);

        /// <summary>
        /// Der aktuell eingeloggte User
        /// </summary>
        User? CurrentUser { get; }

        /// <summary>
        /// Loggt den User aus
        /// </summary>
        void Logout();
    }
}