using Maliev.MaterialService.Tests.Helpers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Data.DbContext;
using Maliev.MaterialService.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
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
        // Clean database for each test (migrations already applied by factory)
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
            // Arrange
            var client = _factory.CreateClient(); // No auth token
            var createRequest = new CreateMaterialRequest { Name = "test", Code = "test" };
    
            // Act
            var response = await client.PostAsJsonAsync("/material/v1/materials", createRequest);
    
            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    
        [Fact]
        public async Task ReadOperations_WithoutAuthToken_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(); // No auth token
    
            // Act
            var response = await client.GetAsync("/material/v1/materials");
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WriteOperations_WithInsufficientRole_ReturnsForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = _factory.CreateTestJwtToken(roles: new[] { "Viewer" }); // Create a token with a non-admin role
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var createRequest = new CreateMaterialRequest { Name = "test", Code = "test" };

        // Act
        var response = await client.PostAsJsonAsync("/material/v1/materials", createRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ReadOperations_WithSufficientRole_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = _factory.CreateTestJwtToken(roles: new[] { "Admin" }); // Standard role that should have access
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/material/v1/materials");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}