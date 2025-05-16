using System.Text.Json.Serialization;

namespace SpacecraftWeather.ExternalWebService.DTOs
{
    public class WeatherDto
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double GenerationTimeMs { get; set; }

        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        public string Timezone { get; set; }

        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; }

        public double Elevation { get; set; }

        [JsonPropertyName("hourly_units")]
        public HourlyUnits HourlyUnits { get; set; } = new();

        public Hourly Hourly { get; set; } = new();
    }
}
