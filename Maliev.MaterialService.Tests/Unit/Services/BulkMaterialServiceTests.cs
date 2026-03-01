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
        // Arrange
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

        // Act
        var result = await _bulkService.BulkImportMaterialsAsync(request, "TestUser");

        // Assert
        Assert.Equal(2, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        _materialServiceMock.Verify(s => s.CreateMaterialAsync(It.IsAny<CreateMaterialRequest>(), "TestUser"), Times.Exactly(2));
    }

    [Fact]
    public async Task BulkImportMaterialsAsync_WithFailures_TracksErrors()
    {
        // Arrange
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

        // Act
        var result = await _bulkService.BulkImportMaterialsAsync(request, "TestUser");

        // Assert
        Assert.Equal(1, result.SuccessCount);
        Assert.Equal(1, result.FailureCount);
        var errorForC2 = Assert.Single(result.Errors, e => e.MaterialCode == "C2" && e.Error == "Duplicate code");
        Assert.NotNull(errorForC2);
    }
}
