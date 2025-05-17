using Microsoft.EntityFrameworkCore;

namespace SpacecraftWeather.Infrastructure.Database
{
    public static class Extensions
    {
        public static void AddWeatherDbContext(this WebApplicationBuilder builder)
        {

            builder.Services.AddDbContext<WeatherDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
