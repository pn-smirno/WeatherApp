using System;

namespace WeatherApp.Models
{
    /// <summary>
    /// Температура по часам (для графика)
    /// </summary>
    public class HourlyTemperature
    {
        public DateTime Hour { get; set; }      // Время
        public double Temperature { get; set; } // Температура в этот час
    }
}