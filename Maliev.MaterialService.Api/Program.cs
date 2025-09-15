using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using HealthChecks.UI.Client;
using Maliev.MaterialService.Api.Authentication;
using Maliev.MaterialService.Api.Configurations;
using Maliev.MaterialService.Api.HealthChecks;
using Maliev.MaterialService.Api.Middleware;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Api.Services;
using Maliev.MaterialService.Data.DbContexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using System.ComponentModel.DataAnnotations;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.RateLimiting;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Maliev Material Service");

    // Load secrets.yaml
    builder.Configuration.AddYamlFile("secrets.yaml", optional: true, reloadOnChange: true);

    // Load secrets from mounted Kubernetes secrets
    var secretsPath = "/mnt/secrets";
    if (Directory.Exists(secretsPath))
    {
        builder.Configuration.AddKeyPerFile(directoryPath: secretsPath, optional: true);
    }

    // API Versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    // Add controllers
    builder.Services.AddControllers();

    // Configure Material DbContext
    if (builder.Environment.IsEnvironment("Testing"))
    {
        builder.Services.AddDbContext<MaterialDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
    }
    else
    {
        builder.Services.AddDbContext<MaterialDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("MaterialDbContext"));
        });
    }

    // Configure caching
    var cacheOptions = new CacheOptions();
    builder.Configuration.GetSection("Cache").Bind(cacheOptions);
    builder.Services.AddSingleton(cacheOptions);

    builder.Services.AddMemoryCache();

    // Configure rate limiting
    var rateLimitOptions = new RateLimitOptions();
    builder.Configuration.GetSection(RateLimitOptions.SectionName).Bind(rateLimitOptions);
    
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Global rate limit
        options.AddPolicy("GlobalPolicy", context =>
            RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitOptions.GlobalPolicy.PermitLimit,
                    Window = TimeSpan.Parse(rateLimitOptions.GlobalPolicy.Window),
                    SegmentsPerWindow = rateLimitOptions.GlobalPolicy.SegmentsPerWindow,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = rateLimitOptions.GlobalPolicy.QueueLimit
                }));

        // Materials endpoint specific rate limit
        options.AddPolicy("MaterialsPolicy", context =>
            RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = rateLimitOptions.MaterialsPolicy.PermitLimit,
                    Window = TimeSpan.Parse(rateLimitOptions.MaterialsPolicy.Window),
                    SegmentsPerWindow = rateLimitOptions.MaterialsPolicy.SegmentsPerWindow,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = rateLimitOptions.MaterialsPolicy.QueueLimit
                }));
    });

    // Configure mapping services
    builder.Services.AddScoped<IManufacturingProcessMappingService, ManufacturingProcessMappingService>();
    builder.Services.AddScoped<IMaterialGroupMappingService, MaterialGroupMappingService>();

    // Configure database initialization service
    builder.Services.AddScoped<DatabaseInitializationService>();

    // Configure Swagger
    builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection(CacheOptions.SectionName));
    builder.Services.AddOptions<CacheOptions>()
        .Bind(builder.Configuration.GetSection(CacheOptions.SectionName))
        .ValidateDataAnnotations();

    // Configure Swagger options
    builder.Services.Configure<SwaggerOptions>(builder.Configuration.GetSection(SwaggerOptions.SectionName));
    builder.Services.AddOptions<SwaggerOptions>()
        .Bind(builder.Configuration.GetSection(SwaggerOptions.SectionName))
        .ValidateDataAnnotations();

    // Configure rate limit options
    builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection(RateLimitOptions.SectionName));
    builder.Services.AddOptions<RateLimitOptions>()
        .Bind(builder.Configuration.GetSection(RateLimitOptions.SectionName))
        .ValidateDataAnnotations();

    // Configure JWT options
    builder.Services.AddOptions<JwtOptions>()
        .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
        .ValidateDataAnnotations();

    // Configure CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            policy =>
            {
                var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>();
                if (corsOrigins != null && corsOrigins.Length > 0)
                {
                    policy.WithOrigins(corsOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                }
                else
                {
                    // Fallback to hardcoded origins if not configured
                    policy.WithOrigins(
                        "https://maliev.com",
                        "https://*.maliev.com",
                        "http://maliev.com",
                        "http://*.maliev.com")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                }
            });
    });

    // Configure Authentication (different schemes for different environments)
    if (builder.Environment.IsEnvironment("Testing"))
    {
        // Testing environment: Use no authentication
        builder.Services.AddAuthentication("Test")
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", _ => { });
    }
    else
    {
        var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
        if (jwtSection.Exists())
        {
            // Production/Staging: Use JWT authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtOptions = new JwtOptions
                    {
                        Issuer = "default-issuer",
                        Audience = "default-audience",
                        SecurityKey = "default-key"
                    };
                    jwtSection.Bind(jwtOptions);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey))
                    };
                });
        }
        else
        {
            // Development: Use anonymous authentication scheme for local testing
            builder.Services.AddAuthentication("Development")
                .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>("Development", _ => { });

            Log.Warning("JWT configuration not found - using Development authentication scheme for local testing. Configure JWT secrets for production functionality.");
        }
    }

    builder.Services.AddAuthorization();

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    builder.Services.AddHttpClient();

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<MaterialDbContext>("MaterialDbContext", tags: new[] { "readiness" })
        .AddCheck<DatabaseHealthCheck>("Database Health Check", tags: new[] { "readiness" })
        // Add checks for external services that the application depends on
        .AddUrlGroup(new Uri("https://httpbin.org/get"), "External API Service", tags: new[] { "readiness", "external" });

    var app = builder.Build();

    app.UseForwardedHeaders();

    // Add correlation ID middleware early in pipeline
    app.UseCorrelationId();

    // Configure the HTTP request pipeline
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "materials/swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        var swaggerOptions = app.Services.GetRequiredService<IOptions<SwaggerOptions>>().Value;
        
        foreach (var description in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/materials/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
        c.RoutePrefix = swaggerOptions.RoutePrefix;
    });

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseHttpsRedirection();

    app.UseHttpMetrics();
    app.UseRateLimiter();
    app.UseCors();

    // Authentication & Authorization (always configured with appropriate scheme)
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Health check endpoints
    app.MapGet("/materials/liveness", () => "Healthy").AllowAnonymous();

    app.MapHealthChecks("/materials/readiness", new HealthCheckOptions
    {
        Predicate = healthCheck => healthCheck.Tags.Contains("readiness"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    // Detailed health check endpoints
    app.MapHealthChecks("/materials/health/database", new HealthCheckOptions
    {
        Predicate = healthCheck => healthCheck.Tags.Contains("readiness") && 
                                  (healthCheck.Name == "MaterialDbContext" || healthCheck.Name == "Database Health Check"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapHealthChecks("/materials/health/external", new HealthCheckOptions
    {
        Predicate = healthCheck => healthCheck.Tags.Contains("external"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapMetrics("/materials/metrics");

    // Ensure database is created and seeded
    using (var scope = app.Services.CreateScope())
    {
        var databaseInitializationService = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
        try
        {
            await databaseInitializationService.InitializeDatabaseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while initializing the database");
        }
    }

    Log.Information("Maliev Material Service started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>
/// JWT configuration options.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// The configuration section name for JWT options.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Gets or sets the JWT issuer.
    /// </summary>
    [Required]
    public required string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the JWT audience.
    /// </summary>
    [Required]
    public required string Audience { get; set; }

    /// <summary>
    /// Gets or sets the JWT security key.
    /// </summary>
    [Required]
    public required string SecurityKey { get; set; }
}