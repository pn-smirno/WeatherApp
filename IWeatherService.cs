using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    /// <summary>
    /// Интерфейс сервиса погоды
    /// </summary>
    public interface IWeatherService
    {
        Task<WeatherData> GetCurrentWeatherAsync(string cityName);
        Task<List<ForecastDay>> GetForecastAsync(string cityName);
        Task<List<HourlyTemperature>> GetHourlyForecastAsync(string cityName);
    }
}