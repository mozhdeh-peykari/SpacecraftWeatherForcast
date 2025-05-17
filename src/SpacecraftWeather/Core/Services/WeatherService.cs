using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SpacecraftWeather.Application.Entities;
using SpacecraftWeather.Infrastructure.Database;
using SpacecraftWeather.Infrastructure.ExternalWebService;

namespace SpacecraftWeather.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherWebService _weatherWebService;
        private readonly WeatherDbContext _dbContext;
        private readonly IMemoryCache _cache;
        const string CACHE_KEY = nameof(WeatherService);

        public WeatherService(IWeatherWebService weatherWebService,
            WeatherDbContext dbContext,
            IMemoryCache cache)
        {
            _weatherWebService = weatherWebService;
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<IEnumerable<Weather>?> GetHourlyTemperatureAsync(double latitude, double longitude)
        {
            if (_cache.TryGetValue(CACHE_KEY, out IEnumerable<Weather> cachedData))
            {
                return cachedData;
            }

            var wsResponse = await _weatherWebService.GetWeatherAsync(latitude, longitude);

            if (wsResponse == null)
            {
                return await GetWeatherFromDb(latitude, longitude);
            }
            else
            {
                await InsertIntoDb(wsResponse);

                _cache.Set(CACHE_KEY, wsResponse, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));

                return wsResponse;
            }
        }

        private async Task InsertIntoDb(IEnumerable<Weather> wsResponse)
        {
            await _dbContext.AddRangeAsync(wsResponse);
            _dbContext.SaveChanges();
        }

        private async Task<IEnumerable<Weather>?> GetWeatherFromDb(double latitude, double longitude)
        {
            var fromDate = DateTime.Today;
            var list = await _dbContext.Weather.Where(x => x.Latitude == latitude
                                        && x.Longitude == longitude
                                        && x.Date >= fromDate)
                .GroupBy(w => new { w.Latitude, w.Longitude, w.Date })
                .Select(g => g.OrderByDescending(w => w.CreatedOn).FirstOrDefault())
                .ToListAsync();

            return list.Count == 0 ? null : list;
        }
    }
}
