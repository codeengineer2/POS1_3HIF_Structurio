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

        public async Task<Project?> CreateProjectAsync(ProjectRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/projects", content);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                    
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

                if (result != null && result.TryGetValue("pid", out var pidObj) && int.TryParse(pidObj.ToString(), out int pid))
                {
                    return new Project
                    {
                        Id = pid,
                        Name = request.Name,
                        Description = request.Description,
                        Color = request.Color,
                        OwnerUid = request.OwnerUid,
                        Board = new Board
                        {
                            ProjectId = pid,
                            Columns = new List<Column>()
                        }
                    };
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}