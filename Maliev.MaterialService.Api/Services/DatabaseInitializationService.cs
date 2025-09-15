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
                // Log connection information (without password)
                var connectionString = _context.Database.GetConnectionString();
                if (!string.IsNullOrEmpty(connectionString))
                {
                    var safeConnectionString = System.Text.RegularExpressions.Regex.Replace(connectionString, @"Password=[^;]+", "Password=****");
                    _logger.LogInformation("Using connection string: {ConnectionString}", safeConnectionString);
                }

                // Check if we can connect to the database
                var canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    _logger.LogInformation("Successfully connected to the database");
                }
                else
                {
                    _logger.LogError("Failed to connect to the database");
                    throw new InvalidOperationException("Cannot connect to the database");
                }

                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migrations applied successfully");

                // Check if there's any data in the tables
                var materialGroupCount = await _context.MaterialGroups.CountAsync();
                var materialCount = await _context.Materials.CountAsync();
                _logger.LogInformation("Database contains {MaterialGroupCount} material groups and {MaterialCount} materials", materialGroupCount, materialCount);
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