using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Structurio
{
    public class Put_timestamp
    {
        public static async Task UpdateAsync(HttpClient httpClient,
                                             int uid,
                                             int pid,
                                             int zid,
                                             DateTime checkIn,
                                             DateTime checkOut,
                                             string duration)
        {
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
            using HttpResponseMessage resp = await httpClient.PutAsync(
                $"timestamps/{uid}/{pid}/{zid}", content);
            resp.EnsureSuccessStatusCode();
        }

    }
}