using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;
using SpacecraftWeather.Database;
using SpacecraftWeather.ExternalWebService;
using SpacecraftWeather.Services;
using SpacecraftWeather.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<WeatherWebServiceSettings>(builder.Configuration.GetSection(nameof(WeatherWebServiceSettings)));
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IWeatherWebService, WeatherWebService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient(nameof(WeatherWebService), (serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<WeatherWebServiceSettings>>().Value;

    client.BaseAddress = new Uri(settings.Endpoint);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(Policy<HttpResponseMessage>
        .HandleResult(r => !r.IsSuccessStatusCode)
        .Or<HttpRequestException>()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
