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
    public class Post_Abrechnung
    {
        private static readonly JsonSerializerOptions _json =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };


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
