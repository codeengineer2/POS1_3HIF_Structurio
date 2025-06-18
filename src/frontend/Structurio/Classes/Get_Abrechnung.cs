using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Structurio
{
    public class Get_Abrechnung
    {
        //prompt:  Wie kann man die daten als JSON empfangen.
        public static async Task<List<Abrechnung_JSON>> GetAsync(HttpClient httpClient, int uid, int pid)
        {
            Log.Information("Get Abrechnung: Start für User={UserId}, Projekt={ProjectId}", uid, pid);
            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync($"abrechnung/{uid}/{pid}");
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<List<Abrechnung_JSON>>(json, options)!;
                Log.Information("Get_Abrechnung: {Count} Einträge geladen", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Get_Abrechnung: Abrechnung wurde nicht korrekt geladen");
                throw;
            }
           


        }
    }
}
