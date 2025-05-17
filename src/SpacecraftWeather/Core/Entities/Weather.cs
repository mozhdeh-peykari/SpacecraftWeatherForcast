namespace SpacecraftWeather.Application.Entities
{
    public class Weather
    {
        public int Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime Date { get; set; }

        public string HourlyTemperatures { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
