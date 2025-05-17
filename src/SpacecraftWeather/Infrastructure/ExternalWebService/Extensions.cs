using Microsoft.Extensions.Options;
using Polly;
using SpacecraftWeather.Application.Settings;

namespace SpacecraftWeather.Infrastructure.ExternalWebService
{
    public static class Extensions
    {
        public static void AddHttpClientWithPolly(this WebApplicationBuilder builder)
        {

            builder.Services.AddHttpClient(nameof(WeatherWebService), (serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<WeatherWebServiceSettings>>().Value;

                client.BaseAddress = new Uri(settings.Endpoint);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler((serviceProvider, client) =>

                Policy<HttpResponseMessage>
                    .HandleResult(r => !r.IsSuccessStatusCode)
                    .Or<HttpRequestException>()
                    .WaitAndRetryAsync(
                        retryCount: 2,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Random.Shared.NextDouble() * (1.5 - 0.5) + 0.5),
                        onRetry: (outcome, delay, retryNumber, context) =>
                        {
                            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                            logger.LogWarning(
                                $"Retry attempt {retryNumber} after {delay.TotalSeconds}s. Reason: {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                        }))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5)))

            .AddPolicyHandler((serviceProvider, _) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                return Policy<HttpResponseMessage>
                    .HandleResult(r => !r.IsSuccessStatusCode)
                    .Or<HttpRequestException>()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 3,
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (outcome, breakDelay, context) =>
                        {
                            logger.LogWarning($"Circuit broken! Blocking calls for {breakDelay.TotalSeconds}s. Reason: {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                        },
                        onReset: (context) =>
                        {
                            logger.LogInformation("Circuit reset - requests are allowed again");
                        },
                        onHalfOpen: () =>
                        {
                            logger.LogInformation("Circuit half-open: Testing next call...");
                        });
            });
        }
    }
}
