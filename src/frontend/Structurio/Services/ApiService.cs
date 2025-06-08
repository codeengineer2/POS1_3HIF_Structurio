using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Structurio.Classes;
using Structurio.Interfaces;

namespace Structurio.Services
{
    public class ApiService : IApiService
    {
        private static readonly HttpClient client = new();

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var request = new LoginRequest { Email = email, Password = password };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/auth/login", content);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LoginResponse>(body);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/auth/register", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}