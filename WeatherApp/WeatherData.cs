using System;

namespace WeatherApp
{
    public class WeatherData
    {
        public string CityName { get; set; } = "";
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public int Pressure { get; set; }
        public string Description { get; set; } = "";
        public string IconCode { get; set; } = "";
        public DateTime LastUpdated { get; set; }
        public int PressureMmHg => (int)(Pressure * 0.75006);
    }
}
