using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurio.Classes;

namespace Structurio.Interfaces
{
    public interface IAuthService
    {
        Task<bool> TryLogin(string email, string password);
        Task<bool> Register(RegisterRequest request);
        User? CurrentUser { get; }
        void Logout();
    }
}