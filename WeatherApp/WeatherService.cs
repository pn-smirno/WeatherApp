using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace WeatherApp
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiKey = "88cdf8ed6aed2ff3a0f5b2ef8a89db65";

        public async Task<WeatherData> GetCurrentWeatherAsync(string cityName)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={ApiKey}&units=metric&lang=ru";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ошибка API: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return new WeatherData
            {
                CityName = root.GetProperty("name").GetString() ?? cityName,
                Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                Pressure = root.GetProperty("main").GetProperty("pressure").GetInt32(),
                Description = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
                IconCode = root.GetProperty("weather")[0].GetProperty("icon").GetString() ?? "01d",
                LastUpdated = DateTime.Now
            };
        }

        public async Task<List<ForecastDay>> GetForecastAsync(string cityName)
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid={ApiKey}&units=metric&lang=ru";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<ForecastDay>();
            }

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var list = root.GetProperty("list");

            var daily = new Dictionary<string, ForecastDay>();

            foreach (var item in list.EnumerateArray())
            {
                string dtTxt = item.GetProperty("dt_txt").GetString() ?? DateTime.Now.ToString();
                DateTime date = DateTime.Parse(dtTxt);
                string key = date.ToString("yyyy-MM-dd");
                double temp = item.GetProperty("main").GetProperty("temp").GetDouble();
                string desc = item.GetProperty("weather")[0].GetProperty("description").GetString() ?? "";
                string icon = item.GetProperty("weather")[0].GetProperty("icon").GetString() ?? "01d";

                if (!daily.ContainsKey(key))
                {
                    daily[key] = new ForecastDay
                    {
                        Date = date.Date,
                        MinTemperature = temp,
                        MaxTemperature = temp,
                        Description = desc,
                        IconCode = icon
                    };
                }
                else
                {
                    if (temp < daily[key].MinTemperature) daily[key].MinTemperature = temp;
                    if (temp > daily[key].MaxTemperature) daily[key].MaxTemperature = temp;
                }
            }

            return new List<ForecastDay>(daily.Values);
        }

        public async Task<List<HourlyTemp>> GetHourlyAsync(string cityName)
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid={ApiKey}&units=metric&lang=ru";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<HourlyTemp>();
            }

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var list = root.GetProperty("list");

            var result = new List<HourlyTemp>();
            int count = 0;

            foreach (var item in list.EnumerateArray())
            {
                if (count >= 8) break;
                result.Add(new HourlyTemp
                {
                    Hour = DateTime.Parse(item.GetProperty("dt_txt").GetString() ?? DateTime.Now.ToString()),
                    Temperature = item.GetProperty("main").GetProperty("temp").GetDouble()
                });
                count++;
            }
            return result;
        }
    }
}
