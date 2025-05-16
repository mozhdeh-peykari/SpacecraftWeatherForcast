namespace SpacecraftWeather.Settings
{
    public class WeatherWebServiceSettings
    {
        public string Endpoint { get; set; }

        public string Resource { get; set; }

        public IEnumerable<WeatherWebServiceParameter> WeatherWebServiceParameters { get; set; }
    }
}
