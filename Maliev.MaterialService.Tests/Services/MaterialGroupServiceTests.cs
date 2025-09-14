using FluentAssertions;
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

public class MaterialGroupServiceTests : IDisposable
{
    private readonly MaterialDbContext _context;
    private readonly Mock<ILogger<MaterialGroupService>> _mockLogger;
    private readonly IMemoryCache _cache;
    private readonly CacheOptions _cacheOptions;
    private readonly MaterialGroupService _service;

    public MaterialGroupServiceTests()
    {
        var options = new DbContextOptionsBuilder<MaterialDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MaterialDbContext(options);
        _mockLogger = new Mock<ILogger<MaterialGroupService>>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _cacheOptions = new CacheOptions
        {
            MaxCacheSize = 100,
            DefaultExpiration = TimeSpan.FromMinutes(30),
            LongExpiration = TimeSpan.FromHours(2)
        };

        _service = new MaterialGroupService(_context, _cache, _mockLogger.Object, _cacheOptions);
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

        var group1 = new MaterialGroup
        {
            Id = 1,
            MaterialFamilyId = 1,
            Name = "Test Group 1",
            Description = "Test Description 1",
            SortOrder = 1,
            MaterialFamily = family,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        var group2 = new MaterialGroup
        {
            Id = 2,
            MaterialFamilyId = 1,
            Name = "Test Group 2",
            Description = "Test Description 2",
            SortOrder = 2,
            MaterialFamily = family,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        _context.MaterialFamilies.Add(family);
        _context.MaterialGroups.Add(group1);
        _context.MaterialGroups.Add(group2);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllGroupsAsync_ShouldReturnAllGroups()
    {
        // Act
        var result = await _service.GetAllGroupsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<MaterialGroup>>();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetGroupByIdAsync_WithValidId_ShouldReturnGroup()
    {
        // Act
        var result = await _service.GetGroupByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<MaterialGroup>();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Group 1");
        result.MaterialFamily.Should().NotBeNull();
        result.MaterialFamily.Name.Should().Be("Test Family");
    }

    [Fact]
    public async Task GetGroupByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetGroupByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetGroupsByFamilyIdAsync_WithValidFamilyId_ShouldReturnGroups()
    {
        // Act
        var result = await _service.GetGroupsByFamilyIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<MaterialGroup>>();
        result.Should().HaveCount(2);
        result.All(g => g.MaterialFamilyId == 1).Should().BeTrue();
    }

    [Fact]
    public async Task GetGroupsByFamilyIdAsync_WithInvalidFamilyId_ShouldReturnEmptyCollection()
    {
        // Act
        var result = await _service.GetGroupsByFamilyIdAsync(999);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<MaterialGroup>>();
        result.Should().HaveCount(0);
    }

    [Fact]
    public async Task CreateGroupAsync_WithValidGroup_ShouldCreateNewGroup()
    {
        // Arrange
        var newGroup = new MaterialGroup
        {
            MaterialFamilyId = 1,
            Name = "New Test Group",
            Description = "A new group for testing",
            SortOrder = 3,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        var result = await _service.CreateGroupAsync(newGroup);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<MaterialGroup>();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("New Test Group");

        // Verify it was saved to database
        var savedGroup = await _context.MaterialGroups.FindAsync(result.Id);
        savedGroup.Should().NotBeNull();
        savedGroup.Should().BeOfType<MaterialGroup>();
        savedGroup!.Name.Should().Be("New Test Group");
    }

    [Fact]
    public async Task CreateGroupAsync_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var newGroup = new MaterialGroup
        {
            MaterialFamilyId = 1,
            Name = "Test Group 1", // This name already exists in the test data
            Description = "A duplicate group for testing",
            SortOrder = 3,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        Func<Task> act = async () => await _service.CreateGroupAsync(newGroup);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A material group with name 'Test Group 1' already exists in this family.");
    }

    [Fact]
    public async Task UpdateGroupAsync_WithValidGroup_ShouldUpdateExistingGroup()
    {
        // Arrange
        var existingGroup = await _context.MaterialGroups.FindAsync(1);
        existingGroup.Should().NotBeNull();

        existingGroup!.Name = "Updated Group Name";
        existingGroup.Description = "Updated description";
        existingGroup.ModifiedDate = DateTime.UtcNow;

        // Act
        var result = await _service.UpdateGroupAsync(existingGroup);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<MaterialGroup>();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Updated Group Name");
        result.Description.Should().Be("Updated description");

        // Verify it was updated in database
        var updatedGroup = await _context.MaterialGroups.FindAsync(1);
        updatedGroup.Should().NotBeNull();
        updatedGroup.Should().BeOfType<MaterialGroup>();
        updatedGroup!.Name.Should().Be("Updated Group Name");
        updatedGroup.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateGroupAsync_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var nonExistentGroup = new MaterialGroup
        {
            Id = 999, // This ID doesn't exist
            MaterialFamilyId = 1,
            Name = "Non-existent Group",
            Description = "A non-existent group for testing",
            SortOrder = 3,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        Func<Task> act = async () => await _service.UpdateGroupAsync(nonExistentGroup);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Material group with ID 999 not found.");
    }

    [Fact]
    public async Task UpdateGroupAsync_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        // First, get an existing group (ID 1)
        var existingGroup = await _context.MaterialGroups.FindAsync(1);
        existingGroup.Should().NotBeNull();

        // Try to update it with the name of another existing group (ID 2)
        var groupToUpdate = new MaterialGroup
        {
            Id = 1,
            MaterialFamilyId = existingGroup!.MaterialFamilyId,
            Name = "Test Group 2", // This name already exists
            Description = "Updated description",
            SortOrder = 1,
            ModifiedDate = DateTime.UtcNow
        };

        // Act
        Func<Task> act = async () => await _service.UpdateGroupAsync(groupToUpdate);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A material group with name 'Test Group 2' already exists in this family.");
    }

    [Fact]
    public async Task DeleteGroupAsync_WithValidId_ShouldRemoveGroup()
    {
        // Act
        await _service.DeleteGroupAsync(1);

        // Assert
        var deletedGroup = await _context.MaterialGroups.FindAsync(1);
        deletedGroup.Should().BeNull();
    }

    [Fact]
    public async Task DeleteGroupAsync_WithNonExistentId_ShouldThrowKeyNotFoundException()
    {
        // Act
        Func<Task> act = async () => await _service.DeleteGroupAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Material group with ID 999 not found.");
    }

    [Fact]
    public async Task GroupExistsAsync_WithExistingId_ShouldReturnTrue()
    {
        // Act
        var result = await _service.GroupExistsAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GroupExistsAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.GroupExistsAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllFamiliesAsync_ShouldReturnAllFamilies()
    {
        // Act
        var result = await _service.GetAllFamiliesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IEnumerable<MaterialFamily>>();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetFamilyByIdAsync_WithValidId_ShouldReturnFamily()
    {
        // Act
        var result = await _service.GetFamilyByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<MaterialFamily>();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Family");
    }

    [Fact]
    public async Task GetFamilyByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetFamilyByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
        _cache.Dispose();
    }
}