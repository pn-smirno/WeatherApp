using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class OpenWeatherService : IWeatherService
    {
        // ВСТАВЬТЕ ВАШ API КЛЮЧ СЮДА!
        private const string ApiKey = "d7565a5eebddb0eb41b5e1878143416a";

        private readonly HttpClient _httpClient;

        // Базовые URL для разных запросов
        private const string CurrentWeatherUrl = "https://api.openweathermap.org/data/2.5/weather";
        private const string ForecastUrl = "https://api.openweathermap.org/data/2.5/forecast";

        public OpenWeatherService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Получение текущей погоды
        /// </summary>
        public async Task<WeatherData> GetCurrentWeatherAsync(string cityName)
        {
            // Формируем URL запроса (units=metric - для градусов Цельсия)
            string url = $"{CurrentWeatherUrl}?q={cityName}&appid={ApiKey}&units=metric&lang=ru";

            // Отправляем запрос и ждём ответ
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            // Проверяем, успешен ли запрос
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Город '{cityName}' не найден. Проверьте название.");
            }

            // Получаем JSON строку
            string json = await response.Content.ReadAsStringAsync();

            // Парсим JSON в объект JObject
            JObject data = JObject.Parse(json);

            // Извлекаем нужные данные
            var weatherData = new WeatherData
            {
                CityName = data["name"]?.ToString() ?? cityName,
                Temperature = (double)data["main"]["temp"],
                FeelsLike = (double)data["main"]["feels_like"],
                Humidity = (int)data["main"]["humidity"],
                WindSpeed = (double)data["wind"]["speed"],
                Pressure = (int)data["main"]["pressure"],
                Description = data["weather"][0]["description"]?.ToString() ?? "",
                IconCode = data["weather"][0]["icon"]?.ToString() ?? "",
                LastUpdated = DateTime.Now
            };

            return weatherData;
        }

        /// <summary>
        /// Получение прогноза на несколько дней
        /// </summary>
        public async Task<List<ForecastDay>> GetForecastAsync(string cityName)
        {
            string url = $"{ForecastUrl}?q={cityName}&appid={ApiKey}&units=metric&lang=ru";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Не удалось получить прогноз для города '{cityName}'");
            }

            string json = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(json);

            // Словарь для группировки прогнозов по дням
            var dailyForecasts = new Dictionary<string, ForecastDay>();

            // Проходим по всем элементам прогноза (каждые 3 часа)
            foreach (var item in data["list"])
            {
                DateTime dt = DateTime.Parse(item["dt_txt"].ToString());
                string dateKey = dt.ToString("yyyy-MM-dd");

                double temp = (double)item["main"]["temp"];
                string description = item["weather"][0]["description"]?.ToString() ?? "";
                string iconCode = item["weather"][0]["icon"]?.ToString() ?? "";

                if (!dailyForecasts.ContainsKey(dateKey))
                {
                    // Первый раз видим этот день - создаём запись
                    dailyForecasts[dateKey] = new ForecastDay
                    {
                        Date = dt.Date,
                        MinTemperature = temp,
                        MaxTemperature = temp,
                        Description = description,
                        IconCode = iconCode
                    };
                }
                else
                {
                    // Обновляем мин/макс температуру для этого дня
                    var existing = dailyForecasts[dateKey];
                    existing.MinTemperature = Math.Min(existing.MinTemperature, temp);
                    existing.MaxTemperature = Math.Max(existing.MaxTemperature, temp);

                    // Берём описание и иконку из середины дня для красоты
                    if (dt.Hour == 12)
                    {
                        existing.Description = description;
                        existing.IconCode = iconCode;
                    }
                }
            }

            // Возвращаем список, берём первые 5 дней
            return new List<ForecastDay>(dailyForecasts.Values);
        }

        /// <summary>
        /// Получение почасового прогноза (для графика)
        /// </summary>
        public async Task<List<HourlyTemperature>> GetHourlyForecastAsync(string cityName)
        {
            string url = $"{ForecastUrl}?q={cityName}&appid={ApiKey}&units=metric&lang=ru";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<HourlyTemperature>();
            }

            string json = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(json);

            var hourlyTemps = new List<HourlyTemperature>();

            // Берём первые 24 часа (8 записей по 3 часа = 24 часа)
            int count = 0;
            foreach (var item in data["list"])
            {
                if (count >= 8) break; // Ограничиваем 24 часами

                DateTime dt = DateTime.Parse(item["dt_txt"].ToString());
                double temp = (double)item["main"]["temp"];

                hourlyTemps.Add(new HourlyTemperature
                {
                    Hour = dt,
                    Temperature = temp
                });

                count++;
            }

            return hourlyTemps;
        }
    }
}