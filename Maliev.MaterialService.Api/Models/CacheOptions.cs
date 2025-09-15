using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.Models;

/// <summary>
/// Cache configuration options.
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// The configuration section name for cache options.
    /// </summary>
    public const string SectionName = "Cache";

    /// <summary>
    /// Gets or sets the maximum cache size.
    /// </summary>
    [Range(1, 100000)]
    public int MaxCacheSize { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the default cache expiration time.
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets or sets the long cache expiration time.
    /// </summary>
    public TimeSpan LongExpiration { get; set; } = TimeSpan.FromHours(2);
}