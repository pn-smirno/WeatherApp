using System;

namespace WeatherApp.Models
{
    /// <summary>
    /// Прогноз на один день
    /// </summary>
    public class ForecastDay
    {
        public DateTime Date { get; set; }             // Дата
        public double MinTemperature { get; set; }     // Минимальная температура
        public double MaxTemperature { get; set; }     // Максимальная температура
        public string Description { get; set; }        // Описание погоды
        public string IconCode { get; set; }           // Код иконки
    }
}