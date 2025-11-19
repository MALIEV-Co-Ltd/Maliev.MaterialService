using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Maliev.MaterialService.Api.Services.Cache;

/// <summary>
/// Redis implementation of cache service
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    /// <summary>
    /// Initializes a new instance of RedisCacheService
    /// </summary>
    /// <param name="cache">Distributed cache instance</param>
    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _cache.GetStringAsync(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
        };
        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, options);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}
