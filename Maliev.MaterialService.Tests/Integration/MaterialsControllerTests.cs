using Maliev.MaterialService.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Maliev.MaterialService.Application.DTOs;
using Maliev.MaterialService.Application.DTOs.Materials;
using Maliev.MaterialService.Infrastructure.Persistence;
using Maliev.MaterialService.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Maliev.MaterialService.Tests.Integration;

[Collection("Sequential")]
public class MaterialsControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;

    public MaterialsControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        // This scope and DB context will be used for test setup
        _scope = _factory.Services.CreateScope();
        var dbContext = _scope.ServiceProvider.GetRequiredService<MaterialDbContext>();

        // Clean database for each test (migrations already applied by factory)
        _factory.CleanDatabaseAsync().GetAwaiter().GetResult();
        SeedData.Initialize(dbContext);

        // Authorize the client
        var token = _factory.CreateTestJwtToken(permissions: new[] {
            "material.materials.create",
            "material.materials.read",
            "material.materials.update",
            "material.materials.delete"
        });
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void Dispose()
    {
        // Clean up the scope after each test
        _scope.Dispose();
    }

    [Fact]
    public async Task PostGetUpdateDelete_Material_Lifecycle_HappyPath()
    {
        // Arrange: Create a dedicated client with all necessary permissions for this specific test
        var clientForLifecycle = _factory.CreateAuthenticatedClient(permissions: new[] {
            "material.materials.create",
            "material.materials.read",
            "material.materials.update",
            "material.materials.delete"
        });

        // 1. CREATE a new material
        var createRequest = new CreateMaterialRequest
        {
            Name = $"Test Material-{Guid.NewGuid()}",
            Code = $"TEST-{Guid.NewGuid().ToString().Substring(0, 8)}",
            StockLevel = 100,
            PricePerUnit = 99.99m
        };

        var createResponse = await clientForLifecycle.PostAsJsonAsync("/material/v1/materials", createRequest);

        // Assert CREATE
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdMaterial = await createResponse.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(createdMaterial);
        Assert.Equal(createRequest.Name, createdMaterial.Name);
        Assert.Equal(createRequest.Code, createdMaterial.Code);
        var newId = createdMaterial.Id;

        // 2. GET the created material by ID
        var getResponse = await clientForLifecycle.GetAsync($"/material/v1/materials/{newId}");

        // Assert GET
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetchedMaterial = await getResponse.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(fetchedMaterial);
        Assert.Equal(newId, fetchedMaterial.Id);
        Assert.Equal(createRequest.Name, fetchedMaterial.Name);

        // 3. UPDATE the material
        var updateRequest = new UpdateMaterialRequest
        {
            Name = $"{createRequest.Name}-Updated",
            Code = createRequest.Code,
            StockLevel = 150,
            PricePerUnit = 129.99m
        };

        var updateResponse = await clientForLifecycle.PutAsJsonAsync($"/material/v1/materials/{newId}", updateRequest);

        // Assert UPDATE
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedMaterial = await updateResponse.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(updatedMaterial);
        Assert.Equal(updateRequest.Name, updatedMaterial.Name);
        Assert.Equal(updateRequest.StockLevel, updatedMaterial.StockLevel);

        // Verify the update persisted
        var getAfterUpdateResponse = await clientForLifecycle.GetAsync($"/material/v1/materials/{newId}");
        var fetchedAfterUpdate = await getAfterUpdateResponse.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(fetchedAfterUpdate);
        Assert.Equal(updateRequest.Name, fetchedAfterUpdate.Name);

        // 4. DELETE the material
        var deleteResponse = await clientForLifecycle.DeleteAsync($"/material/v1/materials/{newId}");

        // Assert DELETE
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify it's gone
        var getAfterDeleteResponse = await clientForLifecycle.GetAsync($"/material/v1/materials/{newId}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDeleteResponse.StatusCode);
    }

    [Fact]
    public async Task CreateMaterial_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new CreateMaterialRequest
        {
            Name = "", // Name is required
            Code = $"TEST-{Guid.NewGuid().ToString().Substring(0, 8)}",
            StockLevel = -10, // Stock level cannot be negative
            PricePerUnit = 10.0m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/material/v1/materials", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMaterial_WithDuplicateCode_ReturnsBadRequest()
    {
        // Arrange
        var uniqueCode = "PC-001"; // This code exists in seed data
        var request = new CreateMaterialRequest { Name = "Second Polycarbonate", Code = uniqueCode, StockLevel = 20, PricePerUnit = 10.0m };

        // Act
        var response = await _client.PostAsJsonAsync("/material/v1/materials", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetMaterials_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/material/v1/materials");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMaterials_WithPagination_ReturnsPagedResult()
    {
        // Act
        var response = await _client.GetAsync("/material/v1/materials?page=1&pageSize=2");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<MaterialResponse>>();
        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(3, result.TotalCount); // From seed data
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task GetMaterials_FilterByName_ReturnsMatchingMaterial()
    {
        // Act
        var response = await _client.GetAsync("/material/v1/materials?search=Polycarbonate");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<MaterialResponse>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Polycarbonate", result.Items.First().Name);
    }

    [Fact]
    public async Task GetMaterials_FilterByManufacturingProcess_ReturnsMatchingMaterials()
    {
        // Act
        var response = await _client.GetAsync("/material/v1/materials?manufacturingProcess=CNC Machining");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<MaterialResponse>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Aluminum 6061", result.Items.First().Name);
    }

    [Fact]
    public async Task GetMaterials_FilterByColor_ReturnsMatchingMaterials()
    {
        // Act
        var response = await _client.GetAsync("/material/v1/materials?color=Blue");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<MaterialResponse>>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count());
        Assert.Contains(result.Items, m => m.Name == "Polycarbonate");
        Assert.Contains(result.Items, m => m.Name == "ABS Plastic");
    }

    [Fact]
    public async Task GetMaterials_FilterByMechanicalProperty_ReturnsMatchingMaterial()
    {
        // Act
        var response = await _client.GetAsync("/material/v1/materials?minTensileStrength=50&maxTensileStrength=150");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<MaterialResponse>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Polycarbonate", result.Items.First().Name);
    }

    [Fact]
    public async Task GetMaterialById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/material/v1/materials/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task HealthCheck_Liveness_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/material/liveness");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HealthCheck_Readiness_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/material/readiness");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Metrics_Endpoint_ReturnsMetrics()
    {
        // Act
        var response = await _client.GetAsync("/material/metrics");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }
}
