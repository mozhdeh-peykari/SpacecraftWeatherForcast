using Microsoft.Extensions.Options;
using SpacecraftWeather.Application.Entities;
using SpacecraftWeather.Application.Settings;
using SpacecraftWeather.Infrastructure.ExternalWebService.Dtos;
using System.Text.Json;

namespace SpacecraftWeather.Infrastructure.ExternalWebService
{
    public class WeatherWebService : IWeatherWebService
    {
        private readonly WeatherWebServiceSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherWebService> _logger;

        public WeatherWebService(IOptions<WeatherWebServiceSettings> options,
            IHttpClientFactory httpClientFactory,
            ILogger<WeatherWebService> logger)
        {
            _settings = options.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<Weather>?> GetWeatherAsync(double latitude, double longitude)
        {
            WeatherDto? wsResponse = null;
            try
            {

                var client = _httpClientFactory.CreateClient(nameof(WeatherWebService));

                wsResponse = await client.GetFromJsonAsync<WeatherDto>(
                    $"{_settings.Resource}?latitude={latitude}&longitude={longitude}&hourly=temperature_2m");

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in calling weather web service");
            }

            if (wsResponse != null)
            {
                return MapToDomain(wsResponse);
            }

            return null;
        }

        public List<Weather> MapToDomain(WeatherDto weatherDto)
        {
            ArgumentNullException.ThrowIfNull(weatherDto?.Hourly);

            var hourlyDataByDate = weatherDto.Hourly.Time
                .Select((time, index) => new
                {
                    DateTime.Parse(time).Date,
                    Time = DateTime.Parse(time).ToString("HH:mm"),
                    Temperature = weatherDto.Hourly.Temperature2m[index]
                })
                .GroupBy(x => x.Date);

            var list = new List<Weather>();
            foreach (var dailyData in hourlyDataByDate)
            {
                var hourlyTemperatures = dailyData
                    .ToDictionary(x => x.Time, x => x.Temperature);

                var weather = new Weather
                {
                    Latitude = weatherDto.Latitude,
                    Longitude = weatherDto.Longitude,
                    Date = dailyData.Key,
                    HourlyTemperatures = JsonSerializer.Serialize(hourlyTemperatures),
                    CreatedOn = DateTime.UtcNow
                };

                list.Add(weather);
            }

            return list;
        }
    }
}
