using SpacecraftWeather.Entities;

namespace SpacecraftWeather.Services
{
    public interface IWeatherService
    {
        Task<IEnumerable<Weather>> GetHourlyTemperatureAsync(double latitude, double longitude);
    }
}
