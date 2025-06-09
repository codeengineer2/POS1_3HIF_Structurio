using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;

namespace Structurio
{
    public class Get_timestamp
    {
        public static async Task<List<Timestamp_Json>> GetAsync(HttpClient httpClient, int uid, int pid)
        {
            using HttpResponseMessage response = await httpClient.GetAsync($"timestamps/{uid}/{pid}");
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Timestamp_Json>>(json)!;
        }
    }
}
