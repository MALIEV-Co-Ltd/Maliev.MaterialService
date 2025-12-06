using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Maliev.MaterialService.Api.DTOs.Bulk;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Tests.Fixtures;
using Xunit;

namespace Maliev.MaterialService.Tests.Integration;

public class BulkOperationsControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;

    public BulkOperationsControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        // Set JWT authorization header
        var token = _factory.CreateTestJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<BulkImportResponse>();
        Assert.NotNull(result);
        Assert.Equal(2, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
    }

    [Fact]
    public async Task BulkExport_ReturnsMaterials()
    {
        // Act
        var response = await _client.GetAsync("/materials/v1/bulk/export");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<MaterialResponse>>();
        Assert.NotNull(result);
    }
}
