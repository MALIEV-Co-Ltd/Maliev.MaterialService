using Maliev.MaterialService.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Maliev.MaterialService.Api.Services;

/// <summary>
/// Service for database initialization and seeding.
/// </summary>
public class DatabaseInitializationService
{
    private readonly MaterialDbContext _context;
    private readonly ILogger<DatabaseInitializationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseInitializationService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    public DatabaseInitializationService(MaterialDbContext context, ILogger<DatabaseInitializationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the database by applying migrations or creating the database if it doesn't exist.
    /// </summary>
    /// <returns>A task that represents the asynchronous database initialization operation.</returns>
    public async Task InitializeDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("Starting database initialization");

            if (_context.Database.IsRelational())
            {
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migrations applied successfully");
            }
            else
            {
                await _context.Database.EnsureCreatedAsync();
                _logger.LogInformation("Database created successfully");
            }

            _logger.LogInformation("Database initialization completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}