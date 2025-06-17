using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
namespace Structurio
{
    public class Get_Abrechnung
    {
        //prompt:  Wie kann man die daten als JSON empfangen.
        public static async Task<List<Abrechnung_JSON>> GetAsync(HttpClient httpClient, int uid, int pid)
        {
            using HttpResponseMessage response = await httpClient.GetAsync($"abrechnung/{uid}/{pid}");
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<Abrechnung_JSON>>(json,options)!;
        }
    }
}
