using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Maliev.MaterialService.Api.DTOs.Bulk;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Data.DbContext;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Maliev.MaterialService.Tests.Integration;

public class BulkOperationsControllerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BulkOperationsControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Remove existing DbContext registration
                    services.RemoveAll(typeof(DbContextOptions<MaterialDbContext>));

                    // Add DbContext with Testcontainers PostgreSQL
                    services.AddDbContext<MaterialDbContext>(options =>
                    {
                        options.UseNpgsql(_fixture.PostgresConnectionString)
                               .UseSnakeCaseNamingConvention();
                    });

                    // Configure Test Authentication
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });

                // Override configuration for Redis and RabbitMQ
                builder.UseSetting("Redis:Host", _fixture.RedisConnectionString);
                builder.UseSetting("RabbitMq:Host", _fixture.RabbitMqHost);
                builder.UseSetting("RabbitMq:Port", _fixture.RabbitMqPort.ToString());
            });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Test");

        // Ensure database is created
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MaterialDbContext>();
        dbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task BulkImport_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new BulkImportRequest
        {
            Materials = new List<CreateMaterialRequest>
            {
                new CreateMaterialRequest
                {
                    Name = "Bulk Material 1",
                    Code = "BULK-001",
                    PricePerUnit = 10,
                    StockLevel = 100
                },
                new CreateMaterialRequest
                {
                    Name = "Bulk Material 2",
                    Code = "BULK-002",
                    PricePerUnit = 20,
                    StockLevel = 200
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/materials/v1/bulk/import", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BulkImportResponse>();
        result.Should().NotBeNull();
        result!.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(0);
    }

    [Fact]
    public async Task BulkExport_ReturnsMaterials()
    {
        // Act
        var response = await _client.GetAsync("/materials/v1/bulk/export");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<MaterialResponse>>();
        result.Should().NotBeNull();
    }
}
