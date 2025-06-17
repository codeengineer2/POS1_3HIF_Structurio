using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structurio.Classes;
using Structurio.Interfaces;

namespace Structurio.Services
{
    public class AuthService : IAuthService
    {
        private IApiService apiService;
        public User? CurrentUser { get; private set; }

        public AuthService(IApiService apiService)
        {
            this.apiService = apiService;
        }

        public async Task<bool> TryLogin(string email, string password)
        {
            var result = await apiService.LoginAsync(email, password);
            if (result != null && result.Success)
            {
                CurrentUser = result.User;
                return true;
            }
            return false;
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            return await apiService.RegisterAsync(request);
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}