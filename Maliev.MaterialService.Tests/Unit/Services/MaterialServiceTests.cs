using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Api.MappingProfiles;
using Maliev.MaterialService.Api.Services.Cache;
using MaterialServiceImpl = Maliev.MaterialService.Api.Services.Materials.MaterialService;
using Maliev.MaterialService.Data.DbContext;
using Maliev.MaterialService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Maliev.MaterialService.Tests.Unit.Services;

public class MaterialServiceTests
{
    private readonly MaterialDbContext _context;
    private readonly IMapper _mapper;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<MaterialServiceImpl>> _loggerMock;
    private readonly MaterialServiceImpl _materialService;

    public MaterialServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<MaterialDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new MaterialDbContext(options);

        // Setup AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MaterialProfile>();
        });
        _mapper = config.CreateMapper();

        // Setup mocks
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<MaterialServiceImpl>>();

        _materialService = new MaterialServiceImpl(
            _context,
            _mapper,
            _cacheServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateMaterialAsync_WithValidData_CreatesAndReturnsMaterial()
    {
        // Arrange
        var request = new CreateMaterialRequest
        {
            Name = "Test Material",
            Code = "TEST-001",
            Description = "Test Description",
            PricePerUnit = 100.00m,
            StockLevel = 50
        };

        // Act
        var result = await _materialService.CreateMaterialAsync(request, "TestUser");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Material");
        result.Code.Should().Be("TEST-001");
        result.CreatedBy.Should().Be("TestUser");
        result.Active.Should().BeTrue();
    }

    [Fact]
    public async Task CreateMaterialAsync_WithDuplicateCode_ThrowsException()
    {
        // Arrange
        var material = new Material
        {
            Id = Guid.NewGuid(),
            Name = "Existing Material",
            Code = "DUPLICATE-001",
            PricePerUnit = 100,
            StockLevel = 10,
            CreatedBy = "System",
            Active = true
        };
        _context.Materials.Add(material);
        await _context.SaveChangesAsync();

        var request = new CreateMaterialRequest
        {
            Name = "New Material",
            Code = "DUPLICATE-001",
            PricePerUnit = 200,
            StockLevel = 20
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _materialService.CreateMaterialAsync(request, "TestUser"));
    }

    [Fact]
    public async Task GetMaterialByIdAsync_WithExistingId_ReturnsMaterial()
    {
        // Arrange
        var materialId = Guid.NewGuid();
        var material = new Material
        {
            Id = materialId,
            Name = "Test Material",
            Code = "TEST-002",
            PricePerUnit = 150,
            StockLevel = 30,
            CreatedBy = "System",
            Active = true
        };
        _context.Materials.Add(material);
        await _context.SaveChangesAsync();

        // Act
        var result = await _materialService.GetMaterialByIdAsync(materialId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(materialId);
        result.Name.Should().Be("Test Material");
    }

    [Fact]
    public async Task GetMaterialByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _materialService.GetMaterialByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteMaterialAsync_SoftDeletesMaterial()
    {
        // Arrange
        var materialId = Guid.NewGuid();
        var material = new Material
        {
            Id = materialId,
            Name = "To Delete",
            Code = "DELETE-001",
            PricePerUnit = 100,
            StockLevel = 10,
            CreatedBy = "System",
            Active = true
        };
        _context.Materials.Add(material);
        await _context.SaveChangesAsync();

        // Act
        var result = await _materialService.DeleteMaterialAsync(materialId);

        // Assert
        result.Should().BeTrue();
        var deleted = await _context.Materials.FindAsync(materialId);
        deleted.Should().NotBeNull();
        deleted!.Active.Should().BeFalse();
    }
}
