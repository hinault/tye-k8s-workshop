using MvcAppClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MvcAppClient
{
    public class WeatherClient
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly HttpClient client;

        public WeatherClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<WeatherForecast[]> GetWeatherAsync()
        {
            var responseMessage = await this.client.GetAsync("/weatherforecast");
            var stream = await responseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<WeatherForecast[]>(stream, options);
        }
    }
}
