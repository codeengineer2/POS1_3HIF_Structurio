using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
/// @file Post_timestamp.cs
/// @brief HTTP-Client zur Erstellung eines Zeitstempels.


namespace Structurio
{
    /// @class Post_timestamp
    /// @brief Verwaltet das Erstellen eines neuen Check-in-Zeitstempels.
    public class Post_timestamp
    {
        /// @brief Sendet neuen Timestamp an den Server.
        /// @param httpClient Die HTTP-Verbindung.
        /// @param uid Benutzer-ID.
        /// @param checkIn Check-in Zeitpunkt.
        /// @return Der erstellte Zeitstempel.
        public static async Task<Timestamp_Json> CreateAsync(HttpClient httpClient, int uid, DateTime checkIn)
        {
            try
            {
                Log.Information("Post_timestamp: Erstelle Timestamp für User={UserId} um {Time}", uid, checkIn);
                var payload = new
                {
                    uid = uid,
                    datum_in = checkIn.ToString("yyyy-MM-dd"),
                    checkin = checkIn.ToString("HH:mm:ss"),
                    datum_out = DateTime.MinValue.ToString("yyyy-MM-dd"),
                    checkout = DateTime.MinValue.ToString("HH:mm:ss"),
                    duration = "00:00"
                };
                string json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using HttpResponseMessage resp = await httpClient.PostAsync("timestamps", content);
                resp.EnsureSuccessStatusCode();
                string body = await resp.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Timestamp_Json>(body)!;
                Log.Information("[Post_timestamp] Timestamp erstellt mit ZID={Zid}", result.zid);
                return result;
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Post_timestamp: Fehler beim Erstellen des Timestamps");
                throw;
            }
            
        }
    }
}
