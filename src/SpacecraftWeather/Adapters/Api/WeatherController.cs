using Microsoft.AspNetCore.Mvc;
using SpacecraftWeather.Application.Services;

namespace SpacecraftWeather.Adapters.Api
{
    [ApiController]
    [Route("[controller]/[action]")]

    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpPost]
        public async Task<IActionResult> HourlyTemperature(double latitude = 1, double longitude = 2)
        {
            if (latitude > 90 || latitude < -90)
            {
                return BadRequest("Invalid Latitude");
            }

            if (longitude > 180 || longitude < -180)
            {
                return BadRequest("Invalid Longitude");
            }

            var response = await _weatherService.GetHourlyTemperatureAsync(latitude, longitude);

            if (response == null)
            {
                return NoContent();
            }

            return Ok(response);

        }
    }
}
