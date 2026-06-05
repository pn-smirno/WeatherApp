using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;

namespace WeatherApp
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IWeatherService _weatherService;
        private System.Timers.Timer _autoUpdateTimer;

        private WeatherData _currentWeather = new WeatherData();
        public WeatherData CurrentWeather
        {
            get => _currentWeather;
            set { _currentWeather = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ForecastDay> Forecast { get; set; }
        public ObservableCollection<HourlyTemp> HourlyTemps { get; set; }
        public ObservableCollection<string> Cities { get; set; }

        private string _selectedCity = "Москва";
        public string SelectedCity
        {
            get => _selectedCity;
            set { _selectedCity = value; LoadWeather(); OnPropertyChanged(); }
        }

        // Для ввода нового города
        private string _newCityName = "";
        public string NewCityName
        {
            get => _newCityName;
            set { _newCityName = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private string _error;
        public string Error
        {
            get => _error;
            set { _error = value; OnPropertyChanged(); }
        }

        public ISeries[] TemperatureSeries { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public ICommand RefreshCommand { get; }
        public ICommand AddCityCommand { get; }
        public ICommand RemoveCityCommand { get; }
        public ICommand AddCustomCityCommand { get; } // Новая команда

        public MainViewModel(IWeatherService weatherService)
        {
            _weatherService = weatherService;
            Forecast = new ObservableCollection<ForecastDay>();
            HourlyTemps = new ObservableCollection<HourlyTemp>();
            Cities = new ObservableCollection<string> { "Москва", "Санкт-Петербург", "Новосибирск" };

            RefreshCommand = new RelayCommand(() => LoadWeather());
            AddCityCommand = new RelayCommand(AddCity);
            RemoveCityCommand = new RelayCommand(RemoveCity);
            AddCustomCityCommand = new RelayCommand(AddCustomCity); // Новая команда

            _autoUpdateTimer = new System.Timers.Timer(600000);
            _autoUpdateTimer.Elapsed += (s, e) => LoadWeather();
            _autoUpdateTimer.Start();

            LoadWeather();
        }

        // Добавление города из текстового поля
        private void AddCustomCity()
        {
            if (!string.IsNullOrWhiteSpace(NewCityName) && !Cities.Contains(NewCityName))
            {
                Cities.Add(NewCityName);
                SelectedCity = NewCityName;
                NewCityName = ""; // Очищаем поле
            }
        }

        private async void LoadWeather()
        {
            if (string.IsNullOrEmpty(SelectedCity)) return;

            IsLoading = true;
            Error = null;

            try
            {
                var current = await _weatherService.GetCurrentWeatherAsync(SelectedCity);
                var forecast = await _weatherService.GetForecastAsync(SelectedCity);
                var hourly = await _weatherService.GetHourlyAsync(SelectedCity);

                CurrentWeather = current;

                Forecast.Clear();
                foreach (var day in forecast.Take(5))
                    Forecast.Add(day);

                HourlyTemps.Clear();
                foreach (var h in hourly)
                    HourlyTemps.Add(h);

                UpdateChart();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateChart()
        {
            if (HourlyTemps.Count == 0) return;

            var values = new ObservableCollection<ObservableValue>();
            var labels = new System.Collections.Generic.List<string>();

            foreach (var h in HourlyTemps)
            {
                values.Add(new ObservableValue(h.Temperature));
                labels.Add(h.Hour.ToString("HH:mm"));
            }

            TemperatureSeries = new ISeries[]
            {
                new LineSeries<ObservableValue>
                {
                    Values = values,
                    Fill = null,
                    GeometrySize = 8
                } 
            };

            XAxes = new Axis[] { new Axis { Labels = labels, LabelsRotation = 45 } };
            YAxes = new Axis[] { new Axis { Labeler = (v) => $"{v:F0}°" } };

            OnPropertyChanged(nameof(TemperatureSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
        }

        private void AddCity()
        {
            string[] newCities = { "Екатеринбург", "Казань", "Нижний Новгород", "Челябинск", "Омск" };
            foreach (var city in newCities)
            {
                if (!Cities.Contains(city))
                {
                    Cities.Add(city);
                    return;
                }
            }
        }

        private void RemoveCity()
        {
            if (Cities.Count > 1 && Cities.Contains(SelectedCity))
                Cities.Remove(SelectedCity);
        }
    }
}
