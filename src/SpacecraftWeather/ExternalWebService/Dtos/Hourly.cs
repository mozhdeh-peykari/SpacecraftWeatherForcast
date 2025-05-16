using System.Text.Json.Serialization;

namespace SpacecraftWeather.ExternalWebService.DTOs
{
    public class Hourly
    {
        public List<string> Time { get; set; } = [];

        [JsonPropertyName("temperature_2m")]
        public List<double> Temperature2m { get; set; } = [];
    }
}
