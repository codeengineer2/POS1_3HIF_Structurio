using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
/// @file Post_Abrechnung.cs
/// @brief HTTP-Client für das Erstellen von Abrechnungen per POST.

namespace Structurio
{

    /// @class Post_Abrechnung
    /// @brief Sendet neue Abrechnungsdaten im JSON-Format an den Server.
    public class Post_Abrechnung
    {
        private static readonly JsonSerializerOptions _json =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        /// @brief Erstellt einen neuen Abrechnungseintrag auf dem Server.
        /// @param httpClient Die HTTP-Verbindung.
        /// @param uid Benutzer-ID.
        /// @param pid Projekt-ID.
        /// @param name Name der Abrechnung.
        /// @param date Datum.
        /// @param price Preis.
        /// @param category Kategorie.
        /// @param rechnungPath Pfad zur Rechnung.
        /// @return Die erstellte Abrechnung.
        public static async Task<Abrechnung_JSON> CreateAsync(
            HttpClient httpClient,
            int uid,
            int pid,
            string name,
            DateTime date,
            double price,
            string category,
            string rechnungPath)
        {
            try
            {
                Log.Information("Post_Abrechnung: Erstellung gestartet");
                var payload = new
                {
                    uid,
                    pid,
                    name,
                    date = date.ToString("yyyy-MM-dd"),
                    price,
                    category,
                    rechnung = rechnungPath
                };

                string json = JsonSerializer.Serialize(payload, _json);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using HttpResponseMessage resp = await httpClient.PostAsync("abrechnung", content);

                resp.EnsureSuccessStatusCode();


                string body = await resp.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Abrechnung_JSON>(body, _json)!;
                Log.Information("Post_Abrechnung: Abrechnung erfolgreich mit ID={id} erstellt",result.Aid);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Post_Abrechnung: Fehler bei der Erstellung der Abrechnung");
                throw;
            }

        }
    }
}
