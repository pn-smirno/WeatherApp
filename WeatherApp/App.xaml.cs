using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            services.AddSingleton<IWeatherService, WeatherService>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            var provider = services.BuildServiceProvider();
            var window = provider.GetRequiredService<MainWindow>();
            window.Show();
        }
    }
}