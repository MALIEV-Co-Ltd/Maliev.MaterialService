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
            // Create a linked token with a timeout to prevent hanging
            using var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutToken.Token);

            // Simple query to check database connectivity - just ping the database
            await _context.Database.CanConnectAsync(linkedToken.Token);

            // If connection is successful, check if tables exist by querying with a limit
            var materialCount = await _context.Materials.Take(1).ToListAsync(linkedToken.Token);
            var materialGroupCount = await _context.MaterialGroups.Take(1).ToListAsync(linkedToken.Token);

            var data = new Dictionary<string, object>
            {
                { "DatabaseConnection", "Successful" },
                { "MaterialsTableAccessible", "Yes" },
                { "MaterialGroupsTableAccessible", "Yes" }
            };

            _logger.LogDebug("Database health check passed. Database connection successful.");

            return HealthCheckResult.Healthy("Database is accessible.", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database is not accessible.", ex);
        }
    }
}