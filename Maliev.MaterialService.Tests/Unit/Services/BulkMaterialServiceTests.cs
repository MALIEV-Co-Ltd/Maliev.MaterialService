using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maliev.MaterialService.Application.DTOs.Bulk;
using Maliev.MaterialService.Application.DTOs.Materials;
using Maliev.MaterialService.Application.Services;
using Maliev.MaterialService.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Maliev.MaterialService.Tests.Unit.Services;

public class BulkMaterialServiceTests
{
    private readonly Mock<IMaterialService> _materialServiceMock;
    private readonly Mock<ILogger<BulkMaterialService>> _loggerMock;
    private readonly BulkMaterialService _bulkService;

    public BulkMaterialServiceTests()
    {
        _materialServiceMock = new Mock<IMaterialService>();
        _loggerMock = new Mock<ILogger<BulkMaterialService>>();

        _bulkService = new BulkMaterialService(_materialServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task BulkImportMaterialsAsync_WithValidData_CallsCreateMaterial()
    {
        var request = new BulkImportRequest
        {
            Materials = new List<CreateMaterialRequest>
            {
                new CreateMaterialRequest { Name = "Mat1", Code = "C1", PricePerUnit = 10, StockLevel = 100 },
                new CreateMaterialRequest { Name = "Mat2", Code = "C2", PricePerUnit = 20, StockLevel = 200 }
            }
        };

        _materialServiceMock.Setup(s => s.CreateMaterialAsync(It.IsAny<CreateMaterialRequest>(), It.IsAny<string>()))
            .ReturnsAsync((CreateMaterialRequest r, string u) => new MaterialResponse { Name = r.Name, Code = r.Code });

        var result = await _bulkService.BulkImportMaterialsAsync(request, "TestUser");

        Assert.Equal(2, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        _materialServiceMock.Verify(s => s.CreateMaterialAsync(It.IsAny<CreateMaterialRequest>(), "TestUser"), Times.Exactly(2));
    }

    [Fact]
    public async Task BulkImportMaterialsAsync_WithFailures_TracksErrors()
    {
        var request = new BulkImportRequest
        {
            Materials = new List<CreateMaterialRequest>
            {
                new CreateMaterialRequest { Name = "Mat1", Code = "C1", PricePerUnit = 10, StockLevel = 100 },
                new CreateMaterialRequest { Name = "Mat2", Code = "C2", PricePerUnit = 20, StockLevel = 200 }
            }
        };

        _materialServiceMock.Setup(s => s.CreateMaterialAsync(It.Is<CreateMaterialRequest>(r => r.Code == "C1"), It.IsAny<string>()))
            .ReturnsAsync(new MaterialResponse { Name = "Mat1", Code = "C1" });

        _materialServiceMock.Setup(s => s.CreateMaterialAsync(It.Is<CreateMaterialRequest>(r => r.Code == "C2"), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Duplicate code"));

        var result = await _bulkService.BulkImportMaterialsAsync(request, "TestUser");

        Assert.Equal(1, result.SuccessCount);
        Assert.Equal(1, result.FailureCount);
        var errorForC2 = Assert.Single(result.Errors, e => e.MaterialCode == "C2" && e.Error == "Duplicate code");
        Assert.NotNull(errorForC2);
    }

    [Fact]
    public async Task BulkImportMaterialsAsync_WithValidateOnly_DoesNotCreateMaterials()
    {
        var request = new BulkImportRequest
        {
            Materials = new List<CreateMaterialRequest>
            {
                new CreateMaterialRequest { Name = "Mat1", Code = "C1", PricePerUnit = 10, StockLevel = 100 }
            },
            ValidateOnly = true
        };

        var result = await _bulkService.BulkImportMaterialsAsync(request, "TestUser");

        Assert.Equal(1, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        _materialServiceMock.Verify(s => s.CreateMaterialAsync(It.IsAny<CreateMaterialRequest>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task BulkImportMaterialsAsync_WithDryRun_DoesNotCreateMaterials()
    {
        var request = new BulkImportRequest
        {
            Materials = new List<CreateMaterialRequest>
            {
                new CreateMaterialRequest { Name = "Mat1", Code = "C1", PricePerUnit = 10, StockLevel = 100 }
            },
            DryRun = true
        };

        var result = await _bulkService.BulkImportMaterialsAsync(request, "TestUser");

        Assert.Equal(1, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        _materialServiceMock.Verify(s => s.CreateMaterialAsync(It.IsAny<CreateMaterialRequest>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task BulkImportMaterialsAsync_EmptyList_ReturnsZeroCounts()
    {
        var request = new BulkImportRequest
        {
            Materials = new List<CreateMaterialRequest>()
        };

        var result = await _bulkService.BulkImportMaterialsAsync(request, "TestUser");

        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task BulkImportMaterialsAsync_WithAllFailures_TracksAllErrors()
    {
        var request = new BulkImportRequest
        {
            Materials = new List<CreateMaterialRequest>
            {
                new CreateMaterialRequest { Name = "Mat1", Code = "C1", PricePerUnit = 10, StockLevel = 100 },
                new CreateMaterialRequest { Name = "Mat2", Code = "C2", PricePerUnit = 20, StockLevel = 200 }
            }
        };

        _materialServiceMock.Setup(s => s.CreateMaterialAsync(It.IsAny<CreateMaterialRequest>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Validation failed"));

        var result = await _bulkService.BulkImportMaterialsAsync(request, "TestUser");

        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(2, result.FailureCount);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public async Task BulkExportMaterialsAsync_ReturnsAllMaterials()
    {
        var materials = new List<MaterialResponse>
        {
            new MaterialResponse { Id = Guid.NewGuid(), Name = "Mat1", Code = "C1" },
            new MaterialResponse { Id = Guid.NewGuid(), Name = "Mat2", Code = "C2" }
        };

        _materialServiceMock.Setup(s => s.GetAllMaterialsAsync())
            .ReturnsAsync(materials);

        var result = await _bulkService.BulkExportMaterialsAsync();

        Assert.Equal(2, result.Count());
        _materialServiceMock.Verify(s => s.GetAllMaterialsAsync(), Times.Once);
    }

    [Fact]
    public async Task BulkExportMaterialsAsync_WithNoMaterials_ReturnsEmptyList()
    {
        _materialServiceMock.Setup(s => s.GetAllMaterialsAsync())
            .ReturnsAsync(new List<MaterialResponse>());

        var result = await _bulkService.BulkExportMaterialsAsync();

        Assert.Empty(result);
    }
}
