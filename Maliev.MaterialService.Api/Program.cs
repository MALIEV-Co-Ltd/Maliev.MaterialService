using Maliev.MaterialService.Api.Middleware;
using Maliev.MaterialService.Api.Services.Bulk;
using Maliev.MaterialService.Api.Services.Cache;
using Maliev.MaterialService.Api.Services.Materials;
using Maliev.MaterialService.Data.DbContext;
using Maliev.MaterialService.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// --- Secrets & Configuration ---
builder.AddGoogleSecretManagerVolume(); // Load secrets from /mnt/secrets if available

// --- Infrastructure & Observability ---
builder.AddServiceDefaults(); // OpenTelemetry, health checks, resilience
builder.AddServiceMeters("materials-meter"); // Register service meters for OpenTelemetry business metrics

// Core application services
// ServiceDefaults handles testing environment automatically for DatabaseMetricsInterceptor
builder.Services.AddSingleton<DatabaseMetricsInterceptor>();

// Cache service - test setup/fixture handles environment-specific configuration
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// Add PostgreSQL DbContext - test setup handles environment-specific configuration via connection string override
builder.AddPostgresDbContext<MaterialDbContext>(connectionStringName: "MaterialDbContext"); // PostgreSQL with retry logic

// Redis Distributed Cache (ServiceDefaults) - enforce Redis on all environments
builder.AddRedisDistributedCache(instanceName: "material:");
// Register CacheWarmingService
builder.Services.AddHostedService<Maliev.MaterialService.Api.BackgroundServices.CacheWarmingService>();

// --- API Configuration ---
builder.AddDefaultCors(); // CORS from CORS:AllowedOrigins config
builder.AddDefaultApiVersioning(); // API versioning with URL segment reader

// JWT Authentication (tests override via PostConfigureAll with dynamic RSA keys)
builder.AddJwtAuthentication();

// Add OpenAPI (must be in Program.cs for XML comments to work via source generator)
if (!builder.Environment.IsProduction())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi("v1", options =>
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info = new()
            {
                Title = "Material Service API",
                Version = "v1",
                Description = "Material inventory management service. Provides CRUD operations for materials with supplier associations, bulk import/export capabilities, supplier validation, reference data for categories and units, and cached responses for high-performance lookups.",
                Contact = new() { Name = "Maliev Support", Email = "support@maliev.com" }
            };
            return Task.CompletedTask;
        });
    });
}

builder.Services.AddHttpClient<Maliev.MaterialService.Api.Services.External.ISupplierServiceClient, Maliev.MaterialService.Api.Services.External.SupplierServiceClient>()
    .AddStandardResilienceHandler();

builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IBulkMaterialService, BulkMaterialService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Customer", policy => policy.RequireRole("Customer"));
    options.AddPolicy("Employee", policy => policy.RequireRole("Employee"));
    options.AddPolicy("Manager", policy => policy.RequireRole("Manager"));
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EmployeeOrHigher", policy => policy.RequireRole("Employee", "Manager", "Admin"));
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.Services.AddControllers();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Run database migrations on startup (skip in Testing environment)
if (!app.Environment.IsEnvironment("Testing"))
{
    try
    {
        await app.MigrateDatabaseAsync<MaterialDbContext>();
    }
    catch (Exception ex)
    {
        Log.MigrationFailed(logger, ex);
        // Don't throw - allow app to start for debugging
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseCors();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Map endpoints after middleware
app.MapControllers();

// Map Aspire default endpoints (/health, /alive, /metrics)
app.MapDefaultEndpoints(servicePrefix: "material");

// Map OpenAPI and Scalar documentation (dev/staging only)
app.MapApiDocumentation(servicePrefix: "material");

Log.ServiceStarted(logger);
await app.RunAsync();

/// <summary>
/// Program entry point for Material Service API
/// </summary>
public partial class Program
{
    internal static partial class Log
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "MaterialService started successfully")]
        public static partial void ServiceStarted(ILogger logger);

        [LoggerMessage(Level = LogLevel.Error, Message = "Database migration failed - application may not function correctly")]
        public static partial void MigrationFailed(ILogger logger, Exception exception);
    }
}