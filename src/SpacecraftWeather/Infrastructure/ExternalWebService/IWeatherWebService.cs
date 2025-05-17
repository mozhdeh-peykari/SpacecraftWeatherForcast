using SpacecraftWeather.Application.Entities;

namespace SpacecraftWeather.Infrastructure.ExternalWebService
{
    public interface IWeatherWebService
    {
        Task<IEnumerable<Weather>> GetWeatherAsync(double latitude, double longitude);
    }
}
