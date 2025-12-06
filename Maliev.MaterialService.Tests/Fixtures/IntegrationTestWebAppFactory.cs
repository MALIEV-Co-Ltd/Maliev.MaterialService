using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Maliev.MaterialService.Data.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Testcontainers.PostgreSql;

namespace Maliev.MaterialService.Tests.Fixtures;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly RSA _testRsa;
    private const string TestIssuer = "test-issuer";
    private const string TestAudience = "test-audience";

    public IntegrationTestWebAppFactory()
    {
        // Generate ephemeral RSA key for test JWT tokens
        _testRsa = RSA.Create(2048);

        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:18")
            .WithDatabase("material_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start PostgreSQL container (RabbitMQ and Redis use in-memory fallbacks in Testing environment)
        await _postgresContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        _testRsa.Dispose();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment FIRST to ensure it's available during configuration
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Clear existing sources and add test configuration with highest priority
            config.Sources.Clear();
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "RABBITMQ_ENABLED", "false" },
                { "ASPNETCORE_ENVIRONMENT", "Testing" },
                { "Jwt:Issuer", TestIssuer },
                { "Jwt:Audience", TestAudience },
                { "Jwt:PublicKey", "dummy-key-will-be-replaced-by-test-rsa" }
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove existing DbContext registration
            services.RemoveAll<DbContextOptions<MaterialDbContext>>();
            services.RemoveAll<MaterialDbContext>();

            // Add DbContext with Testcontainers connection string
            services.AddDbContext<MaterialDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString())
                       .UseSnakeCaseNamingConvention();
            });

            // Configure existing JWT Bearer options for tests
            services.PostConfigure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(
                Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = null; // Disable authority discovery for tests
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = TestIssuer,
                        ValidAudience = TestAudience,
                        IssuerSigningKey = new RsaSecurityKey(_testRsa),
                        ClockSkew = TimeSpan.Zero // No clock skew for tests
                    };
                });

            services.AddAuthorization();

            // Build service provider and apply migrations
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MaterialDbContext>();
            dbContext.Database.Migrate();
        });
    }

    /// <summary>
    /// Creates a test JWT token with specified claims for integration testing.
    /// </summary>
    /// <param name="userId">User ID claim</param>
    /// <param name="roles">User roles</param>
    /// <param name="additionalClaims">Additional claims to include</param>
    /// <returns>JWT token string</returns>
    public string CreateTestJwtToken(string userId = "test-user", string[]? roles = null, Dictionary<string, string>? additionalClaims = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add roles
        roles ??= new[] { "Admin" };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add additional claims
        if (additionalClaims != null)
        {
            foreach (var (key, value) in additionalClaims)
            {
                claims.Add(new Claim(key, value));
            }
        }

        var credentials = new SigningCredentials(
            new RsaSecurityKey(_testRsa),
            SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
