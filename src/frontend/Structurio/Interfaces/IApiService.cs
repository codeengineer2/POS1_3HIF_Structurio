using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurio.Classes;

namespace Structurio.Interfaces
{
    public interface IApiService
    {
        Task<LoginResponse?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(RegisterRequest request);
    }
}