using Microsoft.AspNetCore.Mvc;
using SpacecraftWeather.Entities;
using SpacecraftWeather.Services;

namespace SpacecraftWeather.Api
{
    [ApiController]
    [Route("[controller]/[action]")]

    public class WeatherController
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpPost]
        public async Task<IEnumerable<Weather>> GetHourlyTemperature(double latitude = 1, double longitude = 2)
        {
            return await _weatherService.GetHourlyTemperatureAsync(latitude, longitude);
        }
    }
}
