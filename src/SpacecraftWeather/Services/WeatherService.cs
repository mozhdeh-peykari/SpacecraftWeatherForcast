using Microsoft.EntityFrameworkCore;
using SpacecraftWeather.Database;
using SpacecraftWeather.Entities;
using SpacecraftWeather.ExternalWebService;

namespace SpacecraftWeather.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherWebService _weatherWebService;
        private readonly WeatherDbContext _dbContext;

        public WeatherService(IWeatherWebService weatherWebService,
            WeatherDbContext dbContext)
        {
            _weatherWebService = weatherWebService;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Weather>?> GetHourlyTemperatureAsync(double latitude, double longitude)
        {
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
                return wsResponse;
            }

            //
        }
    }
}
