using Maliev.Aspire.ServiceDefaults;
using Maliev.MaterialService.Api.Services.Auth;
using Maliev.MaterialService.Application.Services;
using Maliev.MaterialService.Infrastructure.Persistence;
using Maliev.MaterialService.Infrastructure.Persistence.Interceptors;
using Maliev.MaterialService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
// Initialize bootstrap logging
using var loggerFactory = LoggerFactory.Create(logBuilder => logBuilder.AddConsole());
var bootstrapLogger = loggerFactory.CreateLogger("Program");

try
{
    Log.StartingHost(bootstrapLogger, "Material Service");

    var builder = WebApplication.CreateBuilder(args);

    // --- Secrets & Configuration ---
    builder.AddGoogleSecretManagerVolume(); // Load secrets from /mnt/secrets if available

    // --- Infrastructure & Observability ---
    builder.AddServiceDefaults(); // OpenTelemetry, health checks, resilience
    builder.AddStandardMiddleware(options =>
    {
        options.EnableRequestLogging = true;
    });
    builder.AddServiceMeters("materials-meter"); // Register service meters for OpenTelemetry business metrics

    builder.AddIAMServiceClient("material");
    builder.Services.AddIAMRegistration<MaterialIAMRegistrationService>("material");

    // Core application services
    builder.Services.AddSingleton<AuthMetrics>();
    builder.Services.AddSingleton<Maliev.Aspire.ServiceDefaults.Authorization.IAuthMetrics>(sp =>
        sp.GetRequiredService<AuthMetrics>());
    builder.Services.AddSingleton<DatabaseMetricsInterceptor>();

    builder.AddMassTransitWithRabbitMq(); // Standard messaging integration

    // Add PostgreSQL DbContext - test setup handles environment-specific configuration via connection string override
    builder.AddPostgresDbContext<MaterialDbContext>(connectionName: "MaterialDbContext"); // PostgreSQL with retry logic

    // Redis Distributed Cache (ServiceDefaults) - enforce Redis on all environments
    builder.AddStandardCache("material:"); // Redis + in-memory fallback, memory-optimized
    // Register CacheWarmingService
    builder.Services.AddHostedService<Maliev.MaterialService.Infrastructure.BackgroundServices.CacheWarmingService>();

    // --- API Configuration ---
    builder.AddStandardCors(); // CORS with fail-fast validation
    builder.AddDefaultApiVersioning(); // API versioning with URL segment reader

    // JWT Authentication (tests override via PostConfigureAll with dynamic RSA keys)
    builder.AddJwtAuthentication();

    // Authorization
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider,
        Maliev.Aspire.ServiceDefaults.Authorization.PermissionAuthorizationPolicyProvider>();
    builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler,
        Maliev.Aspire.ServiceDefaults.Authorization.PermissionAuthorizationHandler>();
    builder.Services.AddAuthorizationBuilder();

    // Add OpenAPI (must be in Program.cs for XML comments to work via source generator)
    if (!builder.Environment.IsProduction())
    {
        builder.AddStandardOpenApi(
            title: "MALIEV Material Service API",
            description: "Material inventory management service. Provides CRUD operations for materials with supplier associations, bulk import/export capabilities, supplier validation, reference data for categories and units, and cached responses for high-performance lookups.");
    }

    builder.AddServiceClient<Maliev.MaterialService.Application.Services.ISupplierServiceClient, Maliev.MaterialService.Infrastructure.Services.SupplierServiceClient>("SupplierService");

    builder.Services.AddScoped<IMaterialService, Maliev.MaterialService.Infrastructure.Services.MaterialService>();
    builder.Services.AddScoped<IBulkMaterialService, Maliev.MaterialService.Infrastructure.Services.BulkMaterialService>();

    builder.Services.AddPermissionAuthorization();

    builder.AddStandardRateLimiting(); // Memory-optimized for low-spec nodes
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });

    builder.Services.AddControllers(options =>
    {
        // Register RequirePermissionAttribute if needed globally or just ensure it's handled
        // Actually, IAsyncAuthorizationFilter should be picked up from attributes.
    });

    var app = builder.Build();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    // Run database migrations on startup (skip in Testing environment)
    await app.MigrateDatabaseAsync<MaterialDbContext>();

    app.UseStandardMiddleware();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }
    app.UseResponseCompression();
    app.UseCors();
    app.UseRateLimiter();

    // App Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Map endpoints after middleware
    app.MapControllers();

    // Map Aspire default endpoints (/health, /alive, /metrics)
    app.MapDefaultEndpoints(servicePrefix: "material");

    // Map OpenAPI and Scalar documentation (dev/staging only)
    app.MapApiDocumentation(servicePrefix: "material");

    Log.ServiceStarted(logger, "Material Service");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.HostTerminated(bootstrapLogger, ex, "Material Service");
    // Force flush to ensure Aspire captures the error before process exits
    Console.Out.Flush();
    Console.Error.Flush();
    throw;
}
finally
{
    loggerFactory.Dispose();
}

/// <summary>
/// Program entry point for Material Service API
/// </summary>
public partial class Program
{
    internal static partial class Log
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Starting {ServiceName} host")]
        public static partial void StartingHost(ILogger logger, string serviceName);

        [LoggerMessage(Level = LogLevel.Critical, Message = "{ServiceName} host terminated unexpectedly during startup")]
        public static partial void HostTerminated(ILogger logger, Exception ex, string serviceName);

        [LoggerMessage(Level = LogLevel.Information, Message = "{ServiceName} started successfully")]
        public static partial void ServiceStarted(ILogger logger, string serviceName);

        [LoggerMessage(Level = LogLevel.Error, Message = "Database migration failed - application may not function correctly")]
        public static partial void MigrationFailed(ILogger logger, Exception exception);
    }
}
