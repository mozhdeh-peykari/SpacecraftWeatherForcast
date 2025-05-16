using System.Text.Json.Serialization;

namespace SpacecraftWeather.ExternalWebService.DTOs
{
    public class HourlyUnits
    {
        public string Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public string Temperature2m { get; set; }
    }
}
