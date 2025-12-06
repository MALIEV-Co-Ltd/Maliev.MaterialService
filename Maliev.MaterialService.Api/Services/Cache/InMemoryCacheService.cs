using System.Collections.Concurrent;

namespace Maliev.MaterialService.Api.Services.Cache;

/// <summary>
/// In-memory implementation of ICacheService using ConcurrentDictionary.
/// Used as a fallback when Redis is not available (e.g., in Testing environment).
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, object> _cache = new();

    /// <summary>
    /// Retrieves a value from the in-memory cache.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <returns>The cached value if found, otherwise null.</returns>
    public Task<T?> GetAsync<T>(string key)
    {
        if (_cache.TryGetValue(key, out var value) && value is T typedValue)
        {
            return Task.FromResult<T?>(typedValue);
        }
        return Task.FromResult<T?>(default);
    }

    /// <summary>
    /// Stores a value in the in-memory cache.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">Expiration time (ignored in this in-memory implementation).</param>
    /// <returns>A completed task.</returns>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        _cache[key] = value!;
        // Note: Expiration is ignored in this in-memory implementation
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes a value from the in-memory cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <returns>A completed task.</returns>
    public Task RemoveAsync(string key)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
