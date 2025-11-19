using Asp.Versioning;
using HealthChecks.UI.Client;
using Maliev.MaterialService.Api.Middleware;
using Maliev.MaterialService.Api.Services.Bulk;
using Maliev.MaterialService.Api.Services.Cache;
using Maliev.MaterialService.Api.Services.Materials;
using Maliev.MaterialService.Data.DbContext;
using Maliev.MaterialService.Data.Interceptors;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Scalar.AspNetCore;
using Serilog;
using System.Security.Cryptography;
using System.Threading.RateLimiting;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi("v1", options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info = new()
            {
                Title = "Material Service API",
                Version = "v1",
                Description = "API for managing materials, suppliers, and related entities in the Maliev manufacturing system.",
                Contact = new() { Name = "Maliev Support", Email = "support@maliev.com" }
            };
            return Task.CompletedTask;
        });

        // Note: XML documentation is automatically included by Microsoft.AspNetCore.OpenApi
    }
});

var dbConnectionString = builder.Configuration.GetConnectionString("MaterialDbContext")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MaterialDbContext>((sp, options) =>
{
    options.UseNpgsql(dbConnectionString)
           .UseSnakeCaseNamingConvention()
           .AddInterceptors(new DatabaseMetricsInterceptor());
});

var redisHost = builder.Configuration["Redis:Host"] ?? "redis:6379";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisHost;
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddHttpClient<Maliev.MaterialService.Api.Services.External.ISupplierServiceClient, Maliev.MaterialService.Api.Services.External.SupplierServiceClient>();

builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IBulkMaterialService, BulkMaterialService>();

builder.Services.AddHostedService<Maliev.MaterialService.Api.BackgroundServices.CacheWarmingService>();

var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "rabbitmq";
var rabbitPort = int.Parse(builder.Configuration["RabbitMq:Port"] ?? "5672");
var rabbitUser = builder.Configuration["RabbitMq:Username"] ?? "guest";
var rabbitPass = builder.Configuration["RabbitMq:Password"] ?? "guest";
var rabbitVHost = builder.Configuration["RabbitMq:VirtualHost"] ?? "/";

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, (ushort)rabbitPort, rabbitVHost, h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var publicKeyBase64 = builder.Configuration["Jwt:PublicKey"];
        var rsa = RSA.Create();

        if (!string.IsNullOrEmpty(publicKeyBase64))
        {
            try
            {
                var publicKeyPem = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(publicKeyBase64));
                rsa.ImportFromPem(publicKeyPem);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to import JWT public key");
                throw;
            }
        }
    });

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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration["CORS:AllowedOrigins"]?
            .Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddHealthChecks()
    .AddNpgSql(dbConnectionString!)
    .AddRedis(redisHost);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Map OpenAPI at /materials/openapi/v1.json
    app.MapOpenApi("/materials/openapi/{documentName}.json");

    // Map Scalar at default /scalar/v1 path
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Material Service API")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithOpenApiRoutePattern("/materials/openapi/v1.json");
    });

    // Redirect root to Scalar
    app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
    app.MapGet("/materials", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
    app.MapGet("/materials/scalar/v1", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

app.UseSerilogRequestLogging();
app.UseResponseCompression();
app.UseCors();
app.UseHttpMetrics();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapMetrics("/materials/metrics");
app.MapHealthChecks("/materials/liveness", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/materials/readiness", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

/// <summary>
/// Program entry point for Material Service API
/// </summary>
public partial class Program { }
