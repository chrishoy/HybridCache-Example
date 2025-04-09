using HybridCacheExample;
using HybridCacheExample.Weather;
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddOpenWeather(builder.Configuration)
    .AddHybridCache(builder.Configuration);

var apiKey = builder.Configuration["OpenWeather:ApiKey"]!; // Fetch API key
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weather/{city}", async (string city, IWeatherService weatherService) =>
{
    var forecast = await weatherService.GetCurrentWeatherAsync(city, apiKey);
    return forecast is null ? Results.NotFound() : Results.Ok(forecast);
});

app.MapGet("/clear", async (HybridCache hybridCache) =>
{
    await hybridCache.RemoveByTagAsync("weather");
    return Results.Ok();
});

app.MapGet("/clear/{city}", async (string city, HybridCache hybridCache) =>
{
    await hybridCache.RemoveByTagAsync(city);
    return Results.Ok();
});

app.Run();
