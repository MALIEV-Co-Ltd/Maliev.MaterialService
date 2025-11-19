using System;
using System.Threading.Tasks;

namespace Maliev.MaterialService.Api.Services.Cache;

/// <summary>
/// Service for caching operations
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves an item from cache
    /// </summary>
    /// <typeparam name="T">Type of the item</typeparam>
    /// <param name="key">Cache key</param>
    /// <returns>The cached item or default if not found</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Sets an item in cache
    /// </summary>
    /// <typeparam name="T">Type of the item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Item to cache</param>
    /// <param name="expiration">Optional expiration time</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Removes an item from cache
    /// </summary>
    /// <param name="key">Cache key</param>
    Task RemoveAsync(string key);
}
