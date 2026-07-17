using Maliev.MaterialService.Application.Services;
using Maliev.MaterialService.Infrastructure.Persistence;
using Maliev.MaterialService.Tests.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Maliev.MaterialService.Tests.Fixtures;

public class IntegrationTestWebAppFactory : BaseIntegrationTestFactory<Program, MaterialDbContext>
{
    /// <summary>
    /// Protected SupplierService boundary used by integration tests.
    /// </summary>
    public Mock<ISupplierServiceClient> SupplierServiceClientMock { get; } = CreateSupplierServiceClientMock();

    public override MaterialDbContext CreateDbContext()
    {
        var connectionString = GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<MaterialDbContext>();
        ConfigureDbContextOptions(optionsBuilder, connectionString);

        // Pass null for DatabaseMetricsInterceptor in tests
        return new MaterialDbContext(optionsBuilder.Options, metricsInterceptor: null);
    }

    protected override void ConfigureAdditionalServices(IServiceCollection services)
    {
        services.RemoveAll<DbContextOptions<MaterialDbContext>>();
        services.RemoveAll<MaterialDbContext>();
        services.RemoveAll<ISupplierServiceClient>();

        services.AddScoped(provider =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<MaterialDbContext>();
            ConfigureDbContextOptions(optionsBuilder, GetConnectionString());
            return new MaterialDbContext(optionsBuilder.Options, metricsInterceptor: null);
        });
        services.AddSingleton(SupplierServiceClientMock.Object);
    }

    private static void ConfigureDbContextOptions(
        DbContextOptionsBuilder<MaterialDbContext> optionsBuilder,
        string connectionString)
    {
        optionsBuilder
            .UseNpgsql(connectionString)
            .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
    }

    private string GetConnectionString()
    {
        // Get connection string from environment variable set by BaseIntegrationTestFactory
        return _postgresContainer?.GetConnectionString()
            ?? Environment.GetEnvironmentVariable($"ConnectionStrings__{DbConnectionStringName}")
            ?? throw new InvalidOperationException("Database connection string not found");
    }

    private static Mock<ISupplierServiceClient> CreateSupplierServiceClientMock()
    {
        var mock = new Mock<ISupplierServiceClient>();
        mock.Setup(client => client.ValidateSupplierExistsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return mock;
    }
}
