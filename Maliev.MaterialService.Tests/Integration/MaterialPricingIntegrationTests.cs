using System.Net;
using System.Net.Http.Json;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Tests.Fixtures;
using Xunit;

namespace Maliev.MaterialService.Tests.Integration;

[Collection("Sequential")]
public class MaterialPricingIntegrationTests : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;

    public MaterialPricingIntegrationTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _factory.CleanDatabaseAsync().GetAwaiter().GetResult();

        var token = _factory.CreateTestJwtToken(permissions: new[]
        {
            "material.materials.create",
            "material.materials.read",
            "material.materials.update",
            "material.materials.delete"
        });
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    [Fact]
    public async Task CreateMaterial_WithDensityAndCostPerKg_ReturnsCreatedWithValues()
    {
        var request = new CreateMaterialRequest
        {
            Name = $"PLA Filament-{Guid.NewGuid()}",
            Code = $"PLA-{Guid.NewGuid().ToString().Substring(0, 8)}",
            Density = 1.25m,
            CostPerKg = 500.00m,
            StockLevel = 100,
            PricePerUnit = 25.00m
        };

        var response = await _client.PostAsJsonAsync("/material/v1/materials", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var material = await response.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(material);
        Assert.Equal(1.25m, material.Density);
        Assert.Equal(500.00m, material.CostPerKg);
    }

    [Fact]
    public async Task UpdateMaterial_WithNewDensity_PersistsAndReturnsUpdatedValue()
    {
        var createRequest = new CreateMaterialRequest
        {
            Name = $"ABS Filament-{Guid.NewGuid()}",
            Code = $"ABS-{Guid.NewGuid().ToString().Substring(0, 8)}",
            Density = 1.0m,
            CostPerKg = 400.00m,
            StockLevel = 50,
            PricePerUnit = 30.00m
        };

        var createResponse = await _client.PostAsJsonAsync("/material/v1/materials", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(created);

        var updateRequest = new UpdateMaterialRequest
        {
            Name = created.Name,
            Code = created.Code,
            Density = 1.5m,
            CostPerKg = created.CostPerKg,
            StockLevel = created.StockLevel,
            PricePerUnit = created.PricePerUnit,
            Version = created.Version
        };

        var updateResponse = await _client.PutAsJsonAsync($"/material/v1/materials/{created.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(updated);
        Assert.Equal(1.5m, updated.Density);

        var getResponse = await _client.GetAsync($"/material/v1/materials/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(fetched);
        Assert.Equal(1.5m, fetched.Density);
    }

    [Fact]
    public async Task CreateMaterial_WithDensityOutOfRange_ReturnsBadRequest()
    {
        var request = new CreateMaterialRequest
        {
            Name = $"Invalid Material-{Guid.NewGuid()}",
            Code = $"INV-{Guid.NewGuid().ToString().Substring(0, 8)}",
            Density = 30.0m,
            CostPerKg = 100.00m,
            StockLevel = 10,
            PricePerUnit = 10.00m
        };

        var response = await _client.PostAsJsonAsync("/material/v1/materials", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMaterial_WithCostPerKgOutOfRange_ReturnsBadRequest()
    {
        var request = new CreateMaterialRequest
        {
            Name = $"Invalid Material-{Guid.NewGuid()}",
            Code = $"INV-{Guid.NewGuid().ToString().Substring(0, 8)}",
            Density = 1.0m,
            CostPerKg = 1000000.00m,
            StockLevel = 10,
            PricePerUnit = 10.00m
        };

        var response = await _client.PostAsJsonAsync("/material/v1/materials", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
