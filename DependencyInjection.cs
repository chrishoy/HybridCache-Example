using HybridCacheExample.Weather;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace HybridCacheExample;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenWeather(this IServiceCollection services, IConfigurationManager configuration)
    {
        //// Register services
        var apiKey = configuration["OpenWeather:ApiKey"]!; // Fetch API key
        var apiEndpoint = configuration["OpenWeather:ApiEndpoint"]!; // Fetch API endpoint

        // Add HttpClient which will be used to fetch weather data
        services.AddHttpClient("OpenWeather", client =>
        {
            client.BaseAddress = new Uri(apiEndpoint);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        // Weather service (keyed) and decorated resilient service
        services.AddDecoratedSingleton<IWeatherService, ResilientWeatherService, WeatherService>("OriginalWeatherService");
//        services.AddKeyedSingleton<IWeatherService, WeatherService>("OriginalWeatherService");
//        services.AddSingleton<IWeatherService, ResilientWeatherService>();

        return services;
    }

    public static IServiceCollection AddDecoratedSingleton<TService, TDecoratorService, TDecoratedService>(
        this IServiceCollection services, object key) where TService : class
        where TDecoratedService : class, TService
        where TDecoratorService : class, TService
    {
        // Register the original (keyed) service that will be decorated
        services.AddKeyedSingleton<TService, TDecoratedService>(key);

        // Register the decorator service (the one that will be used to decorate the original service)
        services.AddSingleton<TService, TDecoratorService>();
        return services;
    }


    public static IServiceCollection AddHybridCache(this IServiceCollection services, IConfigurationManager configuration)
    {
        // Add FusionCache using Redis as Hybrid (in-memory + distributed) cache
        string cs = configuration["Redis:Connection"] ?? "localhost:6379"; 
        double mins = double.TryParse(configuration["Redis:CacheDuration"], out mins) ? mins : 5;

        // Add FusionCache using Redis as Hybrid (in-memory + distributed) cache
        services
            .AddFusionCache()
            .WithDefaultEntryOptions(options => options.Duration = TimeSpan.FromMinutes(mins))
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithDistributedCache(new RedisCache(new RedisCacheOptions { Configuration = cs }))
            .AsHybridCache();

        return services;
    }
}
