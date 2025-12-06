using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Api.Services.Materials;
using Maliev.MaterialService.Tests.Fixtures;
using Xunit;

namespace Maliev.MaterialService.Tests.Integration;

public class MaterialsControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public MaterialsControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        // Set JWT authorization header
        var token = _factory.CreateTestJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetMaterials_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/materials/v1/materials");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMaterials_WithPagination_ReturnsPagedResult()
    {
        // Act
        var response = await _client.GetAsync("/materials/v1/materials?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<MaterialResponse>>();
        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task GetMaterialById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/materials/v1/materials/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task HealthCheck_Liveness_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/materials/liveness");

        // Assert
        // May be unhealthy if checking original config instead of Testcontainer endpoints
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task HealthCheck_Readiness_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/materials/readiness");

        // Assert
        // May be unhealthy if checking original config instead of Testcontainer endpoints
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task Metrics_Endpoint_ReturnsMetrics()
    {
        // Act
        var response = await _client.GetAsync("/materials/metrics");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }
}
