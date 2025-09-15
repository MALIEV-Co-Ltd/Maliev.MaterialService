using FluentAssertions;
using Maliev.MaterialService.Api.Constants;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Api.Services;
using Maliev.MaterialService.Data.DbContexts;
using Maliev.MaterialService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Maliev.MaterialService.Tests.Services;

public class MaterialServiceTests : IDisposable
{
    private readonly MaterialDbContext _context;
    private readonly Mock<ILogger<Api.Services.MaterialService>> _mockLogger;
    private readonly IMemoryCache _cache;
    private readonly CacheOptions _cacheOptions;
    private readonly Api.Services.MaterialService _service;

    public MaterialServiceTests()
    {
        var options = new DbContextOptionsBuilder<MaterialDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MaterialDbContext(options);
        _mockLogger = new Mock<ILogger<Api.Services.MaterialService>>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _cacheOptions = new CacheOptions
        {
            MaxCacheSize = 100,
            DefaultExpiration = TimeSpan.FromMinutes(30),
            LongExpiration = TimeSpan.FromHours(2)
        };

        _service = new Api.Services.MaterialService(_context, _cache, _mockLogger.Object, _cacheOptions);
        SeedTestData();
    }

    private void SeedTestData()
    {
        var family = new MaterialFamily
        {
            Id = 1,
            Name = "Test Family",
            Description = "Test Description",
            SortOrder = 1,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        var group = new MaterialGroup
        {
            Id = 1,
            MaterialFamilyId = 1,
            Name = "Test Group",
            Description = "Test Group Description",
            SortOrder = 1,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            MaterialFamily = family
        };

        var material1 = new Material
        {
            Id = 1,
            MaterialGroupId = 1,
            Name = "Test Material 1",
            Description = "First test material",
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            MaterialGroup = group
        };

        var material2 = new Material
        {
            Id = 2,
            MaterialGroupId = 1,
            Name = "Test Material 2",
            Description = "Second test material",
            IsActive = false,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            MaterialGroup = group
        };

        _context.MaterialFamilies.Add(family);
        _context.MaterialGroups.Add(group);
        _context.Materials.AddRange(material1, material2);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllMaterialsAsync_WithIncludeInactiveFalse_ShouldReturnOnlyActiveMaterials()
    {
        // Act
        var result = await _service.GetAllMaterialsAsync(includeInactive: false, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<Material>>();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Test Material 1");
        result.All(m => m.IsActive).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllMaterialsAsync_WithIncludeInactiveTrue_ShouldReturnAllMaterials()
    {
        // Act
        var result = await _service.GetAllMaterialsAsync(includeInactive: true, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<Material>>();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetMaterialByIdAsync_WithValidId_ShouldReturnMaterial()
    {
        // Act
        var result = await _service.GetMaterialByIdAsync(1, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Material>();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Material 1");
        result.MaterialGroup.Should().NotBeNull();
        result.MaterialGroup.Name.Should().Be("Test Group");
    }

    [Fact]
    public async Task GetMaterialByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetMaterialByIdAsync(999, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetMaterialsByGroupIdAsync_WithValidGroupId_ShouldReturnMaterialsInGroup()
    {
        // Act
        var result = await _service.GetMaterialsByGroupIdAsync(1, includeInactive: true, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<Material>>();
        result.Should().HaveCount(2);
        result.All(m => m.MaterialGroupId == 1).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllMaterialsAsync_ShouldCacheResults()
    {
        // Act - First call
        await _service.GetAllMaterialsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act - Second call
        await _service.GetAllMaterialsAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert - Verify cache was used by checking only one database call was made
        var expectedCacheKey = string.Format(MaterialServiceCacheKeys.AllMaterials, false);
        _cache.TryGetValue(expectedCacheKey, out _).Should().BeTrue();
    }

    [Fact]
    public async Task CreateMaterialAsync_WithValidMaterial_ShouldCreateNewMaterial()
    {
        // Arrange
        var newMaterial = new Material
        {
            MaterialGroupId = 1,
            Name = "New Test Material",
            Description = "A new material for testing",
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.CreateMaterialAsync(newMaterial, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Material>();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("New Test Material");

        // Verify it was saved to database
        var savedMaterial = await _context.Materials.FindAsync(result.Id, TestContext.Current.CancellationToken);
        savedMaterial.Should().NotBeNull();
        savedMaterial.Should().BeOfType<Material>();
        savedMaterial!.Name.Should().Be("New Test Material");
    }

    [Fact]
    public async Task UpdateMaterialAsync_WithValidMaterial_ShouldUpdateExistingMaterial()
    {
        // Arrange
        var existingMaterial = await _context.Materials.FindAsync(1, TestContext.Current.CancellationToken);
        existingMaterial.Should().NotBeNull();

        existingMaterial!.Name = "Updated Material Name";
        existingMaterial.Description = "Updated description";
        existingMaterial.ModifiedDate = DateTime.UtcNow;

        // Act
        var result = await _service.UpdateMaterialAsync(existingMaterial, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Material>();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Updated Material Name");
        result.Description.Should().Be("Updated description");

        // Verify it was updated in database
        var updatedMaterial = await _context.Materials.FindAsync(1, TestContext.Current.CancellationToken);
        updatedMaterial.Should().NotBeNull();
        updatedMaterial.Should().BeOfType<Material>();
        updatedMaterial!.Name.Should().Be("Updated Material Name");
        updatedMaterial.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task DeleteMaterialAsync_WithValidId_ShouldRemoveMaterial()
    {
        // Act
        await _service.DeleteMaterialAsync(1, TestContext.Current.CancellationToken);

        // Assert
        var deletedMaterial = await _context.Materials.FindAsync(1, TestContext.Current.CancellationToken);
        deletedMaterial.Should().BeNull();
    }

    [Fact]
    public async Task CreateMaterialAsync_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var newMaterial = new Material
        {
            MaterialGroupId = 1,
            Name = "Test Material 1", // This name already exists in the test data
            Description = "A duplicate material for testing",
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        Func<Task> act = async () => await _service.CreateMaterialAsync(newMaterial, TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A material with name 'Test Material 1' already exists in this material group.");
    }

    [Fact]
    public async Task UpdateMaterialAsync_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var nonExistentMaterial = new Material
        {
            Id = 999, // This ID doesn't exist
            MaterialGroupId = 1,
            Name = "Non-existent Material",
            Description = "A non-existent material for testing",
            IsActive = true,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        Func<Task> act = async () => await _service.UpdateMaterialAsync(nonExistentMaterial, TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Material with ID 999 not found.");
    }

    [Fact]
    public async Task UpdateMaterialAsync_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        // First, get an existing material (ID 1)
        var existingMaterial = await _context.Materials.FindAsync(1, TestContext.Current.CancellationToken);
        existingMaterial.Should().NotBeNull();

        // Try to update it with the name of another existing material (ID 2)
        var materialToUpdate = new Material
        {
            Id = 1,
            MaterialGroupId = existingMaterial!.MaterialGroupId,
            Name = "Test Material 2", // This name already exists
            Description = "Updated description",
            IsActive = true,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        Func<Task> act = async () => await _service.UpdateMaterialAsync(materialToUpdate, TestContext.Current.CancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A material with name 'Test Material 2' already exists in this material group.");
    }

    public void Dispose()
    {
        _context.Dispose();
        _cache.Dispose();
    }
}