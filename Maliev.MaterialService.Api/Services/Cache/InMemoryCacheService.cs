using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Maliev.MaterialService.Api.Services.Cache;

/// <summary>
/// In-memory implementation of ICacheService using the standard IMemoryCache.
/// Used as a fallback when Redis is not available.
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of the InMemoryCacheService class.
    /// </summary>
    /// <param name="cache">The memory cache instance.</param>
    public InMemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <inheritdoc/>
    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    /// <inheritdoc/>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
        };
        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }
}