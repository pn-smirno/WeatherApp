using System;

namespace WeatherApp
{
    public class ForecastDay
    {
        public DateTime Date { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public string Description { get; set; } = "";
        public string IconCode { get; set; } = "";

        public string DisplayDate
        {
            get
            {
                string[] ruDays = { "Воскресенье", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
                return $"{Date:dd.MM} {ruDays[(int)Date.DayOfWeek]}";
            }
        }
    }
}