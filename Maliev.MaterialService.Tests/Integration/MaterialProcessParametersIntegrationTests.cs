using System.Net;
using System.Net.Http.Json;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Tests.Fixtures;
using Xunit;

namespace Maliev.MaterialService.Tests.Integration;

[Collection("Sequential")]
public class MaterialProcessParametersIntegrationTests : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;

    public MaterialProcessParametersIntegrationTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _factory.CleanDatabaseAsync().GetAwaiter().GetResult();

        var token = _factory.CreateTestJwtToken(permissions: new[]
        {
            "material.materials.create",
            "material.materials.read",
            "material.materials.update"
        });
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    [Fact]
    public async Task CreateMaterial_WithFdmProcessParameters_StoresAndReturnsCorrectly()
    {
        var request = new CreateMaterialRequest
        {
            Name = $"PLA Filament-{Guid.NewGuid()}",
            Code = $"PLA-{Guid.NewGuid().ToString().Substring(0, 8)}",
            Density = 1.25m,
            CostPerKg = 500.00m,
            StockLevel = 100,
            PricePerUnit = 25.00m,
            ProcessParameters = new Dictionary<string, string>
            {
                ["FdmVolumetricFlowRate"] = "15",
                ["FdmMinLayerTime"] = "5"
            }
        };

        var response = await _client.PostAsJsonAsync("/material/v1/materials", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var material = await response.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(material);
        Assert.NotNull(material.ProcessParameters);
        Assert.Equal("15", material.ProcessParameters["FdmVolumetricFlowRate"]);
        Assert.Equal("5", material.ProcessParameters["FdmMinLayerTime"]);
    }

    [Fact]
    public async Task CreateMaterial_WithSlaProcessParameters_StoresAndReturnsCorrectly()
    {
        var request = new CreateMaterialRequest
        {
            Name = $"Standard Resin-{Guid.NewGuid()}",
            Code = $"RES-{Guid.NewGuid().ToString().Substring(0, 8)}",
            Density = 1.15m,
            CostPerKg = 800.00m,
            StockLevel = 50,
            PricePerUnit = 40.00m,
            ProcessParameters = new Dictionary<string, string>
            {
                ["SlaLayerExposure"] = "3",
                ["SlaLiftTime"] = "4"
            }
        };

        var response = await _client.PostAsJsonAsync("/material/v1/materials", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var material = await response.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(material);
        Assert.NotNull(material.ProcessParameters);
        Assert.Equal("3", material.ProcessParameters["SlaLayerExposure"]);
        Assert.Equal("4", material.ProcessParameters["SlaLiftTime"]);
    }

    [Fact]
    public async Task GetMaterial_WithoutProcessParameters_ReturnsEmptyDictionary()
    {
        var createRequest = new CreateMaterialRequest
        {
            Name = $"Basic Material-{Guid.NewGuid()}",
            Code = $"BAS-{Guid.NewGuid().ToString().Substring(0, 8)}",
            Density = 1.0m,
            CostPerKg = 100.00m,
            StockLevel = 10,
            PricePerUnit = 10.00m
        };

        var createResponse = await _client.PostAsJsonAsync("/material/v1/materials", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/material/v1/materials/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var material = await getResponse.Content.ReadFromJsonAsync<MaterialResponse>();
        Assert.NotNull(material);
        Assert.NotNull(material.ProcessParameters);
        Assert.Empty(material.ProcessParameters);
    }
}
