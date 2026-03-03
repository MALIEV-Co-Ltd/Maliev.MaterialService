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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Maliev.MaterialService.Tests.Integration;

[Collection("Sequential")]
public class SuppliersControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;

    public SuppliersControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        _scope = _factory.Services.CreateScope();
        var dbContext = _scope.ServiceProvider.GetRequiredService<MaterialDbContext>();

        _factory.CleanDatabaseAsync().GetAwaiter().GetResult();
        SeedData.Initialize(dbContext);

        var token = _factory.CreateTestJwtToken(permissions: new[] {
            "material.suppliers.read"
        });
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    [Fact]
    public async Task GetSupplierReferences_WithExistingSupplier_ReturnsZeroCount()
    {
        var dbContext = _scope.ServiceProvider.GetRequiredService<MaterialDbContext>();
        var supplier = new Domain.Entities.Supplier
        {
            Name = "Test Supplier",
            ContactInfo = "test@supplier.com"
        };
        dbContext.Suppliers.Add(supplier);
        await dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/material/v1/suppliers/{supplier.Id}/references");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SupplierReferenceResponse>();
        Assert.NotNull(result);
        Assert.Equal(0, result.ReferenceCount);
    }

    [Fact]
    public async Task GetSupplierReferences_WithNonExistingSupplier_ReturnsZeroCount()
    {
        var fakeSupplierId = Guid.NewGuid();

        var response = await _client.GetAsync($"/material/v1/suppliers/{fakeSupplierId}/references");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SupplierReferenceResponse>();
        Assert.NotNull(result);
        Assert.Equal(0, result.ReferenceCount);
    }
}

public record SupplierReferenceResponse(int ReferenceCount);

[Collection("Sequential")]
public class ReferenceDataControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;

    public ReferenceDataControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        _scope = _factory.Services.CreateScope();
        var dbContext = _scope.ServiceProvider.GetRequiredService<MaterialDbContext>();

        _factory.CleanDatabaseAsync().GetAwaiter().GetResult();
        SeedData.Initialize(dbContext);

        _factory.ClearCache();

        var token = _factory.CreateTestJwtToken(permissions: new[] {
            "material.categories.read"
        });
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    [Fact]
    public async Task GetColors_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/material/v1/reference/colors");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<ColorResponse>>();
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetManufacturingProcesses_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/material/v1/reference/processes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<ManufacturingProcessResponse>>();
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPostProcessingMethods_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/material/v1/reference/methods");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<PostProcessingMethodResponse>>();
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}

[Collection("Sequential")]
public class InventoryControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;

    public InventoryControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        _scope = _factory.Services.CreateScope();
        var dbContext = _scope.ServiceProvider.GetRequiredService<MaterialDbContext>();

        _factory.CleanDatabaseAsync().GetAwaiter().GetResult();
        SeedData.Initialize(dbContext);

        var token = _factory.CreateTestJwtToken(permissions: new[] {
            "material.inventory.view",
            "material.inventory.count",
            "material.inventory.adjust"
        });
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    [Fact]
    public async Task GetStockLevel_ReturnsStockInfo()
    {
        var materialId = Guid.NewGuid();

        var response = await _client.GetAsync($"/material/v1/inventory/{materialId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task RecordCount_ReturnsSuccess()
    {
        var materialId = Guid.NewGuid();
        var request = new { MaterialId = materialId, Count = 100 };

        var response = await _client.PostAsJsonAsync("/material/v1/inventory/count", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdjustStock_ReturnsSuccess()
    {
        var materialId = Guid.NewGuid();
        var request = new { MaterialId = materialId, Adjustment = 10, Reason = "Received shipment" };

        var response = await _client.PostAsJsonAsync("/material/v1/inventory/adjust", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
