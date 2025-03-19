## Example usage of FusionCache
This is a simple example of how to use `FusionCache` in a .NET Core application to provide L1 and L2 caching.
we'll use this until Microsoft releases the official version of the HybridCache.

`Microsoft.Extensions.Caching.HybridCache` is currently in preview only.

### How to use
- `docker-compose up -d` to start `redis` on port 6379
- `dotnet run` to start the application
- Try the http examples in `HybridCacheExamples.http` (or use Swagger UI)

### Note
- `Fusioncache` allows you to use multiple cache providers, but in this example, we are using only `MemoryCache` and `RedisCache`.
- You can do tag based invalidation with `FusionCache` so you can invalidate ALL cache entries with that tag in one foul swoop.
- The tag is a string, so you can use it to group cache entries together.
- Invalidation via tags does not actually remove the cache entries from the cache, it just marks them as invalid.

- Sign up to OpenWeatherMap and get your API key to use the weather API. Update in `appsettings.json`.
 ```
 {
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "OpenWeather": {
    "ApiKey": "your-api-key-here"
  }
}
```
