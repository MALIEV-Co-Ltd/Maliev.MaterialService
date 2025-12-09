using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Maliev.MaterialService.Data.DbContext;
using Maliev.MaterialService.Data.Entities;
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
        _testRsa = RSA.Create(2048);

        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16") // Use a recent, stable version
            .WithDatabase("material_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
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
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "RABBITMQ_ENABLED", "false" },
                { "ASPNETCORE_ENVIRONMENT", "Testing" },
                { "Jwt:Issuer", TestIssuer },
                { "Jwt:Audience", TestAudience },
                { "Jwt:PublicKey", "dummy-key" }
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<MaterialDbContext>>();
            services.RemoveAll<MaterialDbContext>();

            services.AddDbContext<MaterialDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString())
                       .UseSnakeCaseNamingConvention();
            });
            
            services.PostConfigure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(
                Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = null;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = TestIssuer,
                        ValidAudience = TestAudience,
                        IssuerSigningKey = new RsaSecurityKey(_testRsa)
                    };
                });

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MaterialDbContext>();
            dbContext.Database.Migrate();
            SeedData.Initialize(dbContext); // Seed data for tests
        });
    }

    public string CreateTestJwtToken(string userId = "test-user", string[]? roles = null, Dictionary<string, string>? additionalClaims = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        roles ??= new[] { "Admin" };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims.Select(kvp => new Claim(kvp.Key, kvp.Value)));
        }

        var credentials = new SigningCredentials(new RsaSecurityKey(_testRsa), SecurityAlgorithms.RsaSha256);
        var token = new JwtSecurityToken(TestIssuer, TestAudience, claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

internal static class SeedData
{
    public static void Initialize(MaterialDbContext context)
    {
        if (context.Materials.Any())
        {
            return; // DB has been seeded
        }

        // Lookups
        var proc3dPrinting = new ManufacturingProcess { Name = "3D Printing" };
        var procCncMachining = new ManufacturingProcess { Name = "CNC Machining" };

        var colorRed = new Color { Name = "Red", HexCode = "#FF0000" };
        var colorBlue = new Color { Name = "Blue", HexCode = "#0000FF" };
        var colorSilver = new Color { Name = "Silver", HexCode = "#C0C0C0" };

        var propTensile = new MechanicalProperty { Name = "Tensile Strength", Unit = "MPa" };
        var propHardness = new MechanicalProperty { Name = "Hardness", Unit = "Shore D" };

        context.AddRange(proc3dPrinting, procCncMachining, colorRed, colorBlue, colorSilver, propTensile, propHardness);
        context.SaveChanges();

        // Materials
        var materialA = new Material
        {
            Name = "Polycarbonate",
            Code = "PC-001",
            StockLevel = 100,
            PricePerUnit = 50.0m,
            ManufacturingProcesses = new List<ManufacturingProcess> { proc3dPrinting },
            AvailableColors = new List<Color> { colorRed, colorBlue }
        };

        var materialB = new Material
        {
            Name = "Aluminum 6061",
            Code = "AL-6061",
            StockLevel = 50,
            PricePerUnit = 120.0m,
            ManufacturingProcesses = new List<ManufacturingProcess> { procCncMachining },
            AvailableColors = new List<Color> { colorSilver }
        };

        var materialC = new Material
        {
            Name = "ABS Plastic",
            Code = "ABS-001",
            StockLevel = 200,
            PricePerUnit = 30.0m,
            ManufacturingProcesses = new List<ManufacturingProcess> { proc3dPrinting },
            AvailableColors = new List<Color> { colorBlue }
        };

        context.AddRange(materialA, materialB, materialC);
        context.SaveChanges();

        // Material Properties
        var propsA = new MaterialMechanicalProperty { Material = materialA, MechanicalProperty = propTensile, Value = 100 };
        var propsB_Tensile = new MaterialMechanicalProperty { Material = materialB, MechanicalProperty = propTensile, Value = 310 };
        var propsC_Tensile = new MaterialMechanicalProperty { Material = materialC, MechanicalProperty = propTensile, Value = 40 };
        var propsC_Hardness = new MaterialMechanicalProperty { Material = materialC, MechanicalProperty = propHardness, Value = 75 };

        context.AddRange(propsA, propsB_Tensile, propsC_Tensile, propsC_Hardness);
        context.SaveChanges();
    }
}