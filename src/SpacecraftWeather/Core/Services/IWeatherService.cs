using SpacecraftWeather.Application.Entities;

namespace SpacecraftWeather.Application.Services
{
    public interface IWeatherService
    {
        Task<IEnumerable<Weather>> GetHourlyTemperatureAsync(double latitude, double longitude);
    }
}
