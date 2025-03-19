using HybridCacheExample;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
var apiKey = builder.Configuration["OpenWeather:ApiKey"]!; // Fetch API key
var apiEndpoint = builder.Configuration["OpenWeather:ApiEndpoint"]!; // Fetch API endpoint

// Add HttpClient which will be used to fetch weather data
builder.Services.AddHttpClient("OpenWeather", client =>
{
    client.BaseAddress = new Uri(apiEndpoint);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add FusionCache using Redis as Hybrid (in-memory + distributed) cache
builder.Services
    .AddFusionCache()
    .WithDefaultEntryOptions(options => options.Duration = TimeSpan.FromMinutes(5))
    .WithSerializer(new FusionCacheSystemTextJsonSerializer())
    .WithDistributedCache(new RedisCache(new RedisCacheOptions { Configuration = "localhost:6379" }))
    .AsHybridCache();

builder.Services.AddSingleton<WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weather/{city}", async (string city, WeatherService weatherService) =>
{
    var forecast = await weatherService.GetCurrentWeatherAsync(city, apiKey);
    return forecast is null ? Results.NotFound() : Results.Ok(forecast);
});
//.WithName("GetWeatherForecast")
//.WithOpenApi();

app.MapGet("/clear/{tag}", async (string tag, HybridCache hybridCache) =>
{
    await hybridCache.RemoveByTagAsync(tag);
    return Results.Ok();
});

app.Run();
