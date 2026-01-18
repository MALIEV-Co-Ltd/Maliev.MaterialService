using Maliev.Aspire.ServiceDefaults.Caching;
using Maliev.MaterialService.Api.Mapping;
using Maliev.MaterialService.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Maliev.MaterialService.Api.BackgroundServices;

/// <summary>
/// Background service that warms up Redis cache with reference data on startup
/// </summary>
public class CacheWarmingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CacheWarmingService> _logger;
    private const string ColorsKey = "ref:colors";
    private const string ProcessesKey = "ref:processes";
    private const string MethodsKey = "ref:methods";

    /// <summary>
    /// Initializes a new instance of CacheWarmingService
    /// </summary>
    /// <param name="serviceProvider">Service provider for creating scopes</param>
    /// <param name="logger">Logger instance</param>
    public CacheWarmingService(IServiceProvider serviceProvider, ILogger<CacheWarmingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    /// <summary>
    /// Executes the background service, warming up the Redis cache with reference data.
    /// </summary>
    /// <param name="stoppingToken">Triggered when the application host is performing a graceful shutdown.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cache Warming Service starting...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MaterialDbContext>();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            // Warm up Colors
            var colors = await context.Colors.AsNoTracking().ToListAsync(stoppingToken);
            var colorDtos = colors.Select(c => c.ToColorResponse()).ToList();
            await cacheService.SetAsync(ColorsKey, colorDtos, TimeSpan.FromHours(24));
            _logger.LogInformation("Cached {Count} colors", colors.Count);

            // Warm up Manufacturing Processes
            var processes = await context.ManufacturingProcesses.AsNoTracking().ToListAsync(stoppingToken);
            var processDtos = processes.Select(p => p.ToManufacturingProcessResponse()).ToList();
            await cacheService.SetAsync(ProcessesKey, processDtos, TimeSpan.FromHours(24));
            _logger.LogInformation("Cached {Count} manufacturing processes", processes.Count);

            // Warm up Post Processing Methods
            var methods = await context.PostProcessingMethods.AsNoTracking().ToListAsync(stoppingToken);
            var methodDtos = methods.Select(m => m.ToPostProcessingMethodResponse()).ToList();
            await cacheService.SetAsync(MethodsKey, methodDtos, TimeSpan.FromHours(24));
            _logger.LogInformation("Cached {Count} post processing methods", methods.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during cache warming");
        }

        _logger.LogInformation("Cache Warming Service completed.");
    }
}
