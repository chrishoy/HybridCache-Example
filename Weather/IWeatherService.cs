namespace HybridCacheExample.Weather;

internal interface IWeatherService
{
    Task<WeatherResponse?> GetCurrentWeatherAsync(string cityOrCountry, string apiKey);
}