using System;

namespace WeatherApp.Models
{
    /// <summary>
    /// Главный класс с данными о погоде
    /// </summary>
    public class WeatherData
    {
        public string CityName { get; set; }           // Название города
        public double Temperature { get; set; }        // Текущая температура
        public double FeelsLike { get; set; }          // Ощущается как
        public int Humidity { get; set; }              // Влажность (%)
        public double WindSpeed { get; set; }          // Скорость ветра (м/с)
        public int Pressure { get; set; }              // Давление (гПа)
        public string Description { get; set; }        // Описание (облачно, ясно...)
        public string IconCode { get; set; }           // Код иконки погоды
        public DateTime LastUpdated { get; set; }      // Время последнего обновления

        // Свойство для отображения давления в удобных единицах (мм рт. ст.)
        public int PressureMmHg => (int)(Pressure * 0.75006);
    }
}