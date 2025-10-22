using FlightServiceAPI.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace FlightServiceAPI.Infrastructure.Cache;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.TryGetValue(key, out T? value);
        return ValueTask.FromResult(value);
    }

    public ValueTask SetAsync<T>(string key, T value, TimeSpan? exp = null, CancellationToken cancellationToken = default)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = exp ?? TimeSpan.FromMinutes(10)
        };

        _memoryCache.Set(key, value, cacheOptions);
        return ValueTask.CompletedTask;    
    }

    public ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        return ValueTask.CompletedTask;
        
    }
}