using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Maliev.MaterialService.Data;
using Maliev.MaterialService.Data.Models;
using Maliev.MaterialService.Api.Services;
using Maliev.MaterialService.Api.DTOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading; // Added for CancellationToken
using Microsoft.EntityFrameworkCore.Query; // Added for IAsyncQueryProvider

namespace Maliev.MaterialService.Tests.MaterialGroups
{
    public class MaterialGroupServiceTests
    {
        private readonly MaterialServiceService _service;
        private readonly Mock<MaterialContext> _mockContext;
        private readonly Mock<ILogger<MaterialServiceService>> _mockLogger;

        public MaterialGroupServiceTests()
        {
            _mockContext = new Mock<MaterialContext>();
            _mockLogger = new Mock<ILogger<MaterialServiceService>>();
            _service = new MaterialServiceService(_mockContext.Object, _mockLogger.Object);
        }

        private Mock<DbSet<T>> GetMockDbSet<T>(List<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            var queryableData = data.AsQueryable();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<T>(queryableData.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            // Mock async enumerator for ToListAsync
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestDbAsyncEnumerator<T>(queryableData.GetEnumerator()));

            // Mock Add and Remove operations
            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(s => data.Add(s));
            mockSet.Setup(m => m.Remove(It.IsAny<T>())).Callback<T>(s => data.Remove(s));

            return mockSet;
        }

        [Fact]
        public async Task GetAllMaterialGroupsAsync_ReturnsAllMaterialGroups()
        {
            // Arrange
            var materialGroups = new List<MaterialGroup>
            {
                new MaterialGroup { Id = 1, Name = "Group1", Description = "Desc1" },
                new MaterialGroup { Id = 2, Name = "Group2", Description = "Desc2" }
            };

            _mockContext.Setup(c => c.MaterialGroup).Returns(GetMockDbSet(materialGroups).Object);

            // Act
            var result = await _service.GetAllMaterialGroupsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, mg => mg.Name == "Group1");
            Assert.Contains(result, mg => mg.Name == "Group2");
        }

        [Fact]
        public async Task GetMaterialGroupByIdAsync_ReturnsMaterialGroup_WhenMaterialGroupExists()
        {
            // Arrange
            var materialGroups = new List<MaterialGroup>
            {
                new MaterialGroup { Id = 1, Name = "Group1", Description = "Desc1" }
            };

            _mockContext.Setup(c => c.MaterialGroup).Returns(GetMockDbSet(materialGroups).Object);
            _mockContext.Setup(c => c.MaterialGroup.FindAsync(1)).ReturnsAsync(materialGroups.FirstOrDefault(mg => mg.Id == 1)!);

            // Act
            var result = await _service.GetMaterialGroupByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Group1", result.Name);
        }

        [Fact]
        public async Task GetMaterialGroupByIdAsync_ReturnsNull_WhenMaterialGroupDoesNotExist()
        {
            // Arrange
            var materialGroups = new List<MaterialGroup>();

            _mockContext.Setup(c => c.MaterialGroup).Returns(GetMockDbSet(materialGroups).Object);
            _mockContext.Setup(c => c.MaterialGroup.FindAsync(It.IsAny<int>())).ReturnsAsync((MaterialGroup)null!);

            // Act
            var result = await _service.GetMaterialGroupByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateMaterialGroupAsync_CreatesNewMaterialGroup()
        {
            // Arrange
            var request = new CreateMaterialGroupRequest { Name = "NewGroup", Description = "NewDesc" };
            var materialGroups = new List<MaterialGroup>();
            var mockSet = GetMockDbSet(materialGroups);
            _mockContext.Setup(c => c.MaterialGroup).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.CreateMaterialGroupAsync(request);

            // Assert
            mockSet.Verify(m => m.Add(It.IsAny<MaterialGroup>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("NewGroup", result.Name);
            Assert.Single(materialGroups);
        }

        [Fact]
        public async Task UpdateMaterialGroupAsync_UpdatesExistingMaterialGroup_WhenMaterialGroupExists()
        {
            // Arrange
            var existingMaterialGroup = new MaterialGroup { Id = 1, Name = "OldGroup", Description = "OldDesc" };
            var materialGroups = new List<MaterialGroup> { existingMaterialGroup };
            var mockSet = GetMockDbSet(materialGroups);
            _mockContext.Setup(c => c.MaterialGroup).Returns(mockSet.Object);
            _mockContext.Setup(c => c.MaterialGroup.FindAsync(1)).ReturnsAsync(existingMaterialGroup);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var request = new UpdateMaterialGroupRequest { Id = 1, Name = "UpdatedGroup", Description = "UpdatedDesc" };

            // Act
            var result = await _service.UpdateMaterialGroupAsync(request);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("UpdatedGroup", result.Name);
            Assert.Equal("UpdatedDesc", result.Description);
            Assert.Equal("UpdatedGroup", existingMaterialGroup.Name);
            Assert.Equal("UpdatedDesc", existingMaterialGroup.Description);
        }

        [Fact]
        public async Task UpdateMaterialGroupAsync_ReturnsNull_WhenMaterialGroupDoesNotExist()
        {
            // Arrange
            var request = new UpdateMaterialGroupRequest { Id = 1, Name = "UpdatedGroup", Description = "UpdatedDesc" };
            _mockContext.Setup(c => c.MaterialGroup.FindAsync(It.IsAny<int>())).ReturnsAsync((MaterialGroup)null!); // Added !

            // Act
            var result = await _service.UpdateMaterialGroupAsync(request);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteMaterialGroupAsync_DeletesMaterialGroup_WhenMaterialGroupExists()
        {
            // Arrange
            var existingMaterialGroup = new MaterialGroup { Id = 1, Name = "DeleteGroup", Description = "DeleteDesc" };
            var materialGroups = new List<MaterialGroup> { existingMaterialGroup };
            var mockSet = GetMockDbSet(materialGroups);
            _mockContext.Setup(c => c.MaterialGroup).Returns(mockSet.Object);
            _mockContext.Setup(c => c.MaterialGroup.FindAsync(1)).ReturnsAsync(existingMaterialGroup);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteMaterialGroupAsync(1);

            // Assert
            mockSet.Verify(m => m.Remove(existingMaterialGroup), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.True(result);
            Assert.Empty(materialGroups);
        }

        [Fact]
        public async Task DeleteMaterialGroupAsync_ReturnsFalse_WhenMaterialGroupDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(c => c.MaterialGroup.FindAsync(It.IsAny<int>())).ReturnsAsync((MaterialGroup)null!); // Added !

            // Act
            var result = await _service.DeleteMaterialGroupAsync(1);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
            Assert.False(result);
        }
    }
}