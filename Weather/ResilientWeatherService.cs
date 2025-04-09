namespace HybridCacheExample.Weather;

/// <summary>
/// Decorates original weather service adding retry requests up to 4 times in case of failure.
/// </summary>
internal class ResilientWeatherService : IWeatherService
{
    private readonly IWeatherService _weatherService;

    public ResilientWeatherService(
        [FromKeyedServices("OriginalWeatherService")]IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<WeatherResponse?> GetCurrentWeatherAsync(string cityOrCountry, string apiKey)
    {
        var retryCount = 0;
        while (retryCount < 4)
        {
            try
            {
                return await _weatherService.GetCurrentWeatherAsync(cityOrCountry, apiKey);
            }
            catch (Exception)
            {
                if (retryCount < 4)
                {
                    // Could add a delay here if needed
                    retryCount++;
                }
                else
                {
                    throw;
                }
            }
        }

        // This should never be reached, but just in case
        return null;
    }
}
