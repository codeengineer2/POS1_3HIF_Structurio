using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
/// @file Put_timestamp.cs
/// @brief HTTP-Client zur Aktualisierung eines Zeitstempels.
namespace Structurio
{
    
    /// @class Put_timestamp
    /// @brief Aktualisiert bestehende Einträge für Ein- und Ausstempelzeiten.
    public class Put_timestamp
    {
        /// @brief Aktualisiert einen Zeitstempel anhand der ID und überträgt die Änderungen an die API.
        /// @param httpClient HTTP-Verbindung.
        /// @param uid Benutzer-ID.
        /// @param zid Zeitstempel-ID.
        /// @param checkIn Check-in Zeit.
        /// @param checkOut Check-out Zeit.
        /// @param duration Dauer als String.
        /// @return Kein Rückgabewert.

        public static async Task UpdateAsync(HttpClient httpClient,
                                             int uid,
                                             int zid,
                                             DateTime checkIn,
                                             DateTime checkOut,
                                             string duration)
        {
            Log.Information("Put_timestamp: Update ZID={Zid} auf Checkin={Timein} und Checkout={Timeout} und die Dauer von {duration}", zid, checkIn, checkOut, duration);

            var payload = new
            {
                datum_in = checkIn.ToString("yyyy-MM-dd"),
                checkin = checkIn.ToString("HH:mm:ss"),
                datum_out = checkOut.ToString("yyyy-MM-dd"),
                checkout = checkOut.ToString("HH:mm:ss"),
                duration = duration
            };

            string json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using HttpResponseMessage resp = await httpClient.PutAsync($"timestamps/{uid}/{zid}", content);
            resp.EnsureSuccessStatusCode();
        }

    }
}