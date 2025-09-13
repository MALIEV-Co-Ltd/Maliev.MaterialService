using Maliev.MaterialService.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Maliev.MaterialService.Api.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly MaterialDbContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(MaterialDbContext context, ILogger<DatabaseHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple query to check database connectivity
            var materialCount = await _context.Materials.CountAsync(cancellationToken);
            var materialGroupCount = await _context.MaterialGroups.CountAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                { "MaterialCount", materialCount },
                { "MaterialGroupCount", materialGroupCount }
            };

            _logger.LogDebug("Database health check passed. Materials: {MaterialCount}, Groups: {GroupCount}",
                materialCount, materialGroupCount);

            return HealthCheckResult.Healthy("Database is accessible and contains data.", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database is not accessible.", ex);
        }
    }
}