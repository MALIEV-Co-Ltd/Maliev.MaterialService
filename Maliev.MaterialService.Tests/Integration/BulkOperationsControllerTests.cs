using Maliev.MaterialService.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Maliev.MaterialService.Api.DTOs;
using Maliev.MaterialService.Api.DTOs.Bulk;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Data.DbContext;
using Maliev.MaterialService.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Maliev.MaterialService.Tests.Integration
{
    [Collection("Sequential")]
    public class BulkOperationsControllerTests : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        private readonly IntegrationTestWebAppFactory _factory;
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;

                public BulkOperationsControllerTests(IntegrationTestWebAppFactory factory)
                {
                    _factory = factory;
                    _client = _factory.CreateClient();
        
                    // This scope and DB context will be used for test setup
                    _scope = _factory.Services.CreateScope();
                    var dbContext = _scope.ServiceProvider.GetRequiredService<MaterialDbContext>();

                    // Clean database for each test (migrations already applied by factory)
                    _factory.CleanDatabaseAsync().GetAwaiter().GetResult();
                    SeedData.Initialize(dbContext);
        
                    var token = _factory.CreateTestJwtToken();
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
        public void Dispose()
        {
            _scope.Dispose();
        }

        [Fact]
        public async Task BulkImport_WithValidJsonFile_CreatesMaterials()
        {
            // Arrange
            var initialCount = await GetMaterialCount();
            var jsonContent = """[{"name":"Bulk Material 1","code":"BULK-001","pricePerUnit":10,"stockLevel":100},{"name":"Bulk Material 2","code":"BULK-002","pricePerUnit":20,"stockLevel":200}]""";
            
            using var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(jsonContent));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            using var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(fileContent, "file", "import.json");

                        // Act
                        var response = await _client.PostAsync("/material/v1/bulk/import", multipartContent);
            
                        // Assert
                        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                        var result = await response.Content.ReadFromJsonAsync<BulkImportResponse>();
                        Assert.NotNull(result);
                        Assert.Equal(2, result.SuccessCount);
                        Assert.Equal(0, result.FailureCount);
            
                        var finalCount = await GetMaterialCount();
                        Assert.Equal(initialCount + 2, finalCount);
                    }
            
                    [Fact]
                    public async Task BulkImport_WithDryRun_DoesNotSaveData()
                    {
                        // Arrange
                        var initialCount = await GetMaterialCount();
                        var jsonContent = """[{"name":"DryRun Material","code":"DRY-RUN-001","pricePerUnit":10,"stockLevel":100}]""";
            
                        using var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(jsonContent));
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
                        using var multipartContent = new MultipartFormDataContent();
                        multipartContent.Add(fileContent, "file", "import.json");
            
                        // Act
                        var response = await _client.PostAsync("/material/v1/bulk/import?dryRun=true", multipartContent);
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<BulkImportResponse>();
            Assert.NotNull(result);
            Assert.Equal(1, result.SuccessCount); // Should still report what would have succeeded
            Assert.Equal(0, result.FailureCount);
            
            var finalCount = await GetMaterialCount();
            Assert.Equal(initialCount, finalCount); // The crucial check: no data was saved
        }

        [Fact]
        public async Task BulkImport_WithPartialSuccess_ReturnsCorrectCounts()
        {
            // Arrange
            var initialCount = await GetMaterialCount();
            var jsonContent = """[{"name":"Partial Success","code":"PART-001","pricePerUnit":10,"stockLevel":100},{"name":"","code":"INVALID-001"}]""";
            
            using var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(jsonContent));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            using var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(fileContent, "file", "import.json");

                        // Act
                        var response = await _client.PostAsync("/material/v1/bulk/import", multipartContent);
            
                        // Assert
                        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // Changed to BadRequest due to API design
                        var result = await response.Content.ReadFromJsonAsync<BulkImportResponse>();
                        Assert.NotNull(result);
                        Assert.Equal(1, result.SuccessCount);
                        Assert.Equal(1, result.FailureCount);
                        Assert.Contains("Material name is required.", result.Errors.First().Error);
            
                        var finalCount = await GetMaterialCount();
                        Assert.Equal(initialCount + 1, finalCount);
                    }
        [Fact]
        public async Task BulkExport_AsCsv_ReturnsCsvFile()
        {
            // Act
            var response = await _client.GetAsync("/material/v1/bulk/export?format=csv");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);

            var csvContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Code,Name,PricePerUnit,StockLevel", csvContent); // Check for header
            Assert.Contains("PC-001", csvContent); // Check for seed data
            Assert.Contains("AL-6061", csvContent);
        }

        private async Task<int> GetMaterialCount()
        {
            var response = await _client.GetAsync("/material/v1/materials?pageSize=1");
            var result = await response.Content.ReadFromJsonAsync<PagedResult<MaterialResponse>>();
            return result?.TotalCount ?? 0;
        }
    }
}
