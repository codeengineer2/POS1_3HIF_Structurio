using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
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
            Log.Information("AuthService wurde initialisiert.");

            this.apiService = apiService;
        }

        public async Task<bool> TryLogin(string email, string password)
        {
            Log.Information($"Versuche Login für den Benutzer mit der Email={email}.");

            var result = await apiService.LoginAsync(email, password);
            if (result != null && result.Success)
            {
                Log.Information($"Login erfolgreich für den Benutzer mit der Email={email}.");

                CurrentUser = result.User;
                return true;
            }

            Log.Warning($"Login fehlgeschlagen für den Benutzer mit der Email={email}.");
            return false;
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            Log.Information($"Versuche Registrierung für den neuen Benutzer mit der Email={request.Email}.");

            var success = await apiService.RegisterAsync(request);
            if (success)
            {
                Log.Information($"Registrierung erfolgreich für den neuen Benutzer mit der Email={request.Email}.");
            }
            else
            {
                Log.Warning($"Registrierung fehlgeschlagen für den neuen Benutzer mit der Email={request.Email}.");
            }

            return success;
        }

        public void Logout()
        {
            if (CurrentUser != null)
            {
                Log.Information($"Benutzer mit der Email={CurrentUser.Email} wurde ausgeloggt.");
            }
            else
            {
                Log.Information("Logout aufgerufen aber es war kein Benutzer eingeloggt");
            }

            CurrentUser = null;
        }
    }
}