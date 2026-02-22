using Maliev.MaterialService.Tests.Helpers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Data.DbContext;
using Maliev.MaterialService.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;

namespace Maliev.MaterialService.Tests.Integration;

[Collection("Sequential")]
public class AuthorizationTests : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IntegrationTestWebAppFactory _factory;
    private readonly IServiceScope _scope;

    public AuthorizationTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;

        _scope = _factory.Services.CreateScope();
        var dbContext = _scope.ServiceProvider.GetRequiredService<MaterialDbContext>();
        _factory.CleanDatabaseAsync().GetAwaiter().GetResult();
        SeedData.Initialize(dbContext);
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    [Fact]
    public async Task WriteOperations_WithoutAuthToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var createRequest = new CreateMaterialRequest { Name = "test", Code = "test", PricePerUnit = 10.0m };

        var response = await client.PostAsJsonAsync("/material/v1/materials", createRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ReadOperations_WithoutAuthToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/material/v1/materials");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WriteOperations_WithInsufficientPermission_ReturnsForbidden()
    {
        var client = _factory.CreateAuthenticatedClient(permissions: new[] { "material.materials.read" });

        var createRequest = new CreateMaterialRequest { Name = "test", Code = "test", PricePerUnit = 10.0m };

        var response = await client.PostAsJsonAsync("/material/v1/materials", createRequest);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ReadOperations_WithSufficientPermission_ReturnsOk()
    {
        var client = _factory.CreateAuthenticatedClient(permissions: new[] { "material.materials.read" });

        var response = await client.GetAsync("/material/v1/materials");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminOperations_WithAdminPermission_ReturnsOk()
    {
        var client = _factory.CreateAuthenticatedClient(permissions: new[] { "material.materials.delete" });
        var materialId = Guid.NewGuid();

        var response = await client.DeleteAsync($"/material/v1/materials/{materialId}");

        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ManagerOperations_WithManagerPermission_CanCreateButNotDelete()
    {
        var client = _factory.CreateAuthenticatedClient(permissions: new[] {
            "material.materials.create",
            "material.materials.update",
            "material.materials.read"
        });

        var createRequest = new CreateMaterialRequest { Name = "manager-test", Code = "manager-test", PricePerUnit = 10.0m };

        var createResponse = await client.PostAsJsonAsync("/material/v1/materials", createRequest);
        var deleteResponse = await client.DeleteAsync($"/material/v1/materials/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task AnonymousUser_CannotAccessMaterials()
    {
        var client = _factory.CreateClient();

        var materialsResponse = await client.GetAsync("/material/v1/materials");

        Assert.Equal(HttpStatusCode.Unauthorized, materialsResponse.StatusCode);
    }
}
