using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherApp
{
    public interface IWeatherService
    {
        Task<WeatherData> GetCurrentWeatherAsync(string cityName);
        Task<List<ForecastDay>> GetForecastAsync(string cityName);
        Task<List<HourlyTemp>> GetHourlyAsync(string cityName);
    }
}