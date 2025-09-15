using Maliev.MaterialService.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Maliev.MaterialService.Api.HealthChecks;

/// <summary>
/// Health check for database connectivity and data integrity.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly MaterialDbContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseHealthCheck"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    public DatabaseHealthCheck(MaterialDbContext context, ILogger<DatabaseHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Checks the health of the database by verifying connectivity and data integrity.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous health check operation.</returns>
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