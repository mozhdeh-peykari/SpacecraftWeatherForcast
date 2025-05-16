using SpacecraftWeather.Entities;

namespace SpacecraftWeather.ExternalWebService
{
    public interface IWeatherWebService
    {
        Task<IEnumerable<Weather>> GetWeatherAsync(double latitude, double longitude);
    }
}
