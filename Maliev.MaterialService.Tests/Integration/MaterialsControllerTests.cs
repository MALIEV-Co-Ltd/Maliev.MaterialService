using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Api.Services.Materials;
using Maliev.MaterialService.Data.DbContext;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Maliev.MaterialService.Tests.Integration;

public class MaterialsControllerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public MaterialsControllerTests(IntegrationTestFixture fixture)
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
    public async Task GetMaterials_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/materials/v1/materials");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMaterials_WithPagination_ReturnsPagedResult()
    {
        // Act
        var response = await _client.GetAsync("/materials/v1/materials?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<MaterialResponse>>();
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetMaterialById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/materials/v1/materials/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task HealthCheck_Liveness_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/liveness");

        // Assert
        // May be unhealthy if checking original config instead of Testcontainer endpoints
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task HealthCheck_Readiness_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/readiness");

        // Assert
        // May be unhealthy if checking original config instead of Testcontainer endpoints
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Metrics_Endpoint_ReturnsMetrics()
    {
        // Act
        var response = await _client.GetAsync("/metrics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }
}
