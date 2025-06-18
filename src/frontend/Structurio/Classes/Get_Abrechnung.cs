using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
/// @file Get_Abrechnung.cs
/// @brief HTTP-Client für GET-Abfragen von Abrechnungen.
namespace Structurio
{

    /// @class Get_Abrechnung
    /// @brief Stellt Funktionalität bereit, um Abrechnungsdaten per HTTP abzurufen.
    public class Get_Abrechnung
    {
        //prompt:  Wie kann man die daten als JSON empfangen.

        /// @brief Holt Abrechnungen für einen Benutzer und ein Projekt.
        /// @param httpClient Die HTTP-Verbindung.
        /// @param uid Benutzer-ID.
        /// @param pid Projekt-ID.
        /// @return Liste von Abrechnungen.
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
