using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Structurio
{
    public class Post_timestamp
    {
        public static async Task<Timestamp_Json> CreateAsync(HttpClient httpClient, int uid, DateTime checkIn)
        {
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
            return JsonSerializer.Deserialize<Timestamp_Json>(body)!;
        }
    }
}
