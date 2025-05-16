using Microsoft.EntityFrameworkCore;
using SpacecraftWeather.Entities;

namespace SpacecraftWeather.Database
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Weather> Weather { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Weather>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Weather>()
                .Property(x => x.Latitude).HasPrecision(2);
            modelBuilder.Entity<Weather>()
                .Property(x => x.Longitude).HasPrecision(2);
            modelBuilder.Entity<Weather>()
                .Property(x => x.Date).HasPrecision(2);
            modelBuilder.Entity<Weather>()
                .Property(x => x.CreatedOn).HasPrecision(2);
        }
    }
}
