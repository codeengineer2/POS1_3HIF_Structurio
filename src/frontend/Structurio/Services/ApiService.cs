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
using System.Diagnostics;
using Serilog;

namespace Structurio.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient client;

        public ApiService(HttpClient? injectedClient = null)
        {
            Log.Information("ApiService wurde initialisiert.");
            client = injectedClient ?? new HttpClient();
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            Log.Information($"LoginAsync wurde aufgerufen mit der Emai={email}.");

            var request = new LoginRequest { Email = email, Password = password };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/auth/login", content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning($"Login fehlgeschlagen, StatusCode={response.StatusCode}.");
                    return null;
                }

                Log.Information("Login erfolgreich.");

                var result = JsonConvert.DeserializeObject<LoginResponse>(body);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler in LoginAsync.");
                return null;
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            Log.Information($"RegisterAsync wurde aufgerufen für die Email={request.Email}.");

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/auth/register", content);
                Log.Information($"Registrierung abgeschlossen mit StatusCode={response.StatusCode}.");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler in RegisterAsync.");
                return false;
            }
        }

        public async Task<Project?> CreateProjectAsync(ProjectRequest request)
        {
            Log.Information($"CreateProjectAsync wurde aufgerufen für den Namen={request.Name}.");

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/projects", content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning($"CreateProjectAsync  fehlgeschlagen, StatusCode={response.StatusCode}.");
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

                Log.Information($"Projekt erfolgreich erstellt mit dem Namen={project.Name}.");
                return project;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler in CreateProjectAsync.");
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<Issue?> AddIssueAsync(AddIssueRequest request)
        {
            Log.Information($"AddIssueAsync wurde aufgerufen für die Spalte mit der Beschreibung={request.Description}.");

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/issues", content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning("AddIssueAsync ist fehlgeschlagen, StatusCode={response.StatusCode}.");
                    return null;
                }

                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                if (result != null && result.TryGetValue("issue_id", out var iidObj) && int.TryParse(iidObj.ToString(), out int iid))
                {
                    Log.Information($"Issue wurde erfolgreich erstellt mit der Beschreibung={request.Description}.");

                    return new Issue
                    {
                        Id = iid,
                        Description = request.Description,
                        ColumnId = request.ColumnId
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler in AddIssueAsync.");
                return null;
            }
        }

        public async Task<bool> UpdateIssueAsync(UpdateIssueRequest request)
        {
            Log.Information($"UpdateIssueAsync wurde aufgerufen für das Issue mit der Beschreibung={request.Description}.");

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PutAsync("http://localhost:8080/issues", content);
                Log.Information($"UpdateIssueAsync ist abgeschlossen, StatusCode={response.StatusCode}.");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler in UpdateIssueAsync.");
                return false;
            }
        }

        public async Task<bool> DeleteIssueAsync(int id)
        {
            Log.Information($"DeleteIssueAsync wurde aufgerufen für das Issue mit der ID={id}.");

            try
            {
                var response = await client.DeleteAsync($"http://localhost:8080/issues/{id}");
                Log.Information("Issue wurde gelöscht, StatusCode={response.StatusCode}.");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler in DeleteIssueAsync.");
                return false;
            }
        }

        public async Task<Column?> AddColumnAsync(AddColumnRequest request)
        {
            Log.Information($"AddColumnAsync wurde aufgerufen für das Board mit der ID={request.BoardId}.");

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://localhost:8080/columns", content);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning("AddColumnAsync fehlgeschlagen, StatusCode={response.StatusCode}.");
                    return null;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                if (result != null && result.TryGetValue("cid", out var cidObj) && int.TryParse(cidObj.ToString(), out int cid) && result.TryGetValue("name", out var nameObj))
                {
                    Log.Information($"Spalte wurde erfolgreich erstellt mit der ID={cid}.");

                    return new Column
                    {
                        Id = cid,
                        Name = nameObj.ToString(),
                        Issues = new List<Issue>()
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler in AddColumnAsync.");
                return null;
            }
        }

        public async Task<bool> UpdateColumnAsync(UpdateColumnRequest request)
        {
            Log.Information($"UpdateColumnAsync wurde aufgerufen für ID={request.Id}.");

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PutAsync("http://localhost:8080/columns", content);
                Log.Information("Spalte wurde aktualisiert, StatusCode={response.StatusCode}.");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler in UpdateColumnAsync.");
                return false;
            }
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            Log.Information($"DeleteProjectAsync wurde aufgerufen für das Projekt mit der ID={projectId}.");

            try
            {
                var response = await client.DeleteAsync($"http://localhost:8080/projects/{projectId}");
                Log.Information("Projekt wurde gelöscht, StatusCode={response.StatusCode}.");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler in DeleteProjectAsync.");
                return false;
            }
        }

        public async Task<bool> UpdateProjectAsync(Project project)
        {
            Log.Information($"UpdateProjectAsync wurde aufgerufen für Projekt mit dem Namen={project.Name}.");

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
                Log.Information($"Projekt wurde aktualisiert, StatusCode={response.StatusCode}.");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}, Fehler in UpdateProjectAsync.");
                return false;
            }
        }
    }
}