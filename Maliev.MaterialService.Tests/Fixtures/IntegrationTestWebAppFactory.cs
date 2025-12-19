using Maliev.MaterialService.Data.DbContext;
using Maliev.MaterialService.Tests.Testing;
using Microsoft.EntityFrameworkCore;

namespace Maliev.MaterialService.Tests.Fixtures;

public class IntegrationTestWebAppFactory : BaseIntegrationTestFactory<Program, MaterialDbContext>
{
    public override MaterialDbContext CreateDbContext()
    {
        var connectionString = GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<MaterialDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // Pass null for DatabaseMetricsInterceptor in tests
        return new MaterialDbContext(optionsBuilder.Options, metricsInterceptor: null);
    }

    private string GetConnectionString()
    {
        // Get connection string from environment variable set by BaseIntegrationTestFactory
        return Environment.GetEnvironmentVariable($"ConnectionStrings__{DbConnectionStringName}")
            ?? throw new InvalidOperationException("Database connection string not found");
    }
}
