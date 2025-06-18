using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
/// @file Get_timestamp.cs
/// @brief HTTP-Client für Zeitstempelabrufe.

namespace Structurio
{
    /// @class Get_timestamp
    /// @brief Holt alle Zeitstempel eines Benutzers von der API.
    public class Get_timestamp
    {

        /// @brief Ruft Zeitstempelliste für einen Benutzer ab.
        /// @param httpClient Die HTTP-Verbindung.
        /// @param uid Benutzer-ID.
        /// @return Liste von Zeitstempeln.
        public static async Task<List<Timestamp_Json>> GetAsync(HttpClient httpClient, int uid)
        {
            Log.Information(":Get_timestamp: Für  User={UserId}", uid);
            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync($"timestamps/{uid}");
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<Timestamp_Json>>(json)!;
                Log.Information(":Get_timestamp: {Count} Timestamps geladen", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Get_timestamp: Timestamps konnten nicht aufgerufen werden.");
                throw;
            }

        }
    }
}
