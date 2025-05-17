using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SpacecraftWeather.Database;
using SpacecraftWeather.Entities;
using SpacecraftWeather.ExternalWebService;

namespace SpacecraftWeather.Services
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
                //get from db
                var fromDate = DateTime.Now;
                var list = await _dbContext.Weather.Where(x => x.Latitude == latitude
                                            && x.Longitude == longitude
                                            && x.Date >= fromDate)
                    .GroupBy(w => new { w.Latitude, w.Longitude, w.Date })
                    .Select(g => g.OrderByDescending(w => w.CreatedOn).FirstOrDefault())
                    .ToListAsync();

                return list;
            }
            else
            {
                //insert anyway
                await _dbContext.AddRangeAsync(wsResponse);
                _dbContext.SaveChanges();

                // Save in cache
                _cache.Set(CACHE_KEY, wsResponse, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));

                return wsResponse;
            }
        }
    }
}
