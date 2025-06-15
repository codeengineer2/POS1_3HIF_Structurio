using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Structurio.Classes;
using Structurio.Interfaces;
using Newtonsoft.Json.Linq;
using System.Windows;

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
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                JObject jsonObj;
                jsonObj = JObject.Parse(body);

                var project = new Project
                {
                    Id = (int)jsonObj["pid"],
                    Name = request.Name,
                    Description = request.Description,
                    Color = request.Color,
                    OwnerUid = request.OwnerUid,
                    Board = new Board
                    {
                        Id = (int)jsonObj["board"]["id"],
                        ProjectId = (int)jsonObj["board"]["project_id"],
                        Columns = new List<Column>()
                    }
                };

                return project;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        public async Task<Issue?> AddIssueAsync(AddIssueRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/issues", content);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var body = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

                if (result != null && result.TryGetValue("issue_id", out var iidObj) && int.TryParse(iidObj.ToString(), out int iid))
                {
                    return new Issue
                    {
                        Id = iid,
                        Description = request.Description,
                        ColumnId = request.ColumnId
                    };
                }

                return null;
            }
            catch 
            {
                return null;
            }
        }

        public async Task<bool> UpdateIssueAsync(UpdateIssueRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PutAsync("http://localhost:8080/issues", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteIssueAsync(int id)
        {
            try
            {
                var response = await client.DeleteAsync($"http://localhost:8080/issues/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Column?> AddColumnAsync(AddColumnRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/columns", content);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                if (result != null && result.TryGetValue("cid", out var cidObj) && int.TryParse(cidObj.ToString(), out int cid) && result.TryGetValue("name", out var nameObj))
                {
                    return new Column
                    {
                        Id = cid,
                        Name = nameObj.ToString(),
                        Issues = new List<Issue>()
                    };
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateColumnAsync(UpdateColumnRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PutAsync("http://localhost:8080/columns", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            try
            {
                var response = await client.DeleteAsync($"http://localhost:8080/projects/{projectId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProjectAsync(Project project)
        {
            var json = JsonConvert.SerializeObject(new
            {
                pid = project.Id,
                name = project.Name,
                description = project.Description,
                color = project.Color
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PutAsync("http://localhost:8080/projects", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}