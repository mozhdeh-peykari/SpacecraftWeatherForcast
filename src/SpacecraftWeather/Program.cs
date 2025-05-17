using SpacecraftWeather.Adapters.Middlewares;
using SpacecraftWeather.Application.Services;
using SpacecraftWeather.Application.Settings;
using SpacecraftWeather.Infrastructure.Database;
using SpacecraftWeather.Infrastructure.ExternalWebService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddWeatherDbContext();

builder.Services.Configure<WeatherWebServiceSettings>(builder.Configuration.GetSection(nameof(WeatherWebServiceSettings)));

builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IWeatherWebService, WeatherWebService>();

builder.Services.AddMemoryCache();


builder.AddHttpClientWithPolly();
var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();

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
