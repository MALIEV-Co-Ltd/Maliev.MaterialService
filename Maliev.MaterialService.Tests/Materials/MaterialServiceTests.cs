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

namespace Maliev.MaterialService.Tests.Materials
{
    public class MaterialServiceTests
    {
        private readonly MaterialServiceService _service;
        private readonly Mock<MaterialContext> _mockContext;
        private readonly Mock<ILogger<MaterialServiceService>> _mockLogger;

        public MaterialServiceTests()
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
        public async Task GetAllMaterialsAsync_ReturnsAllMaterials()
        {
            // Arrange
            var materials = new List<Material>
            {
                new Material { Id = 1, Name = "Material1", MaterialGroupId = 1, Machinable = true, Printable = false },
                new Material { Id = 2, Name = "Material2", MaterialGroupId = 1, Machinable = false, Printable = true }
            };

            _mockContext.Setup(c => c.Material).Returns(GetMockDbSet(materials).Object);

            // Act
            var result = await _service.GetAllMaterialsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, m => m.Name == "Material1");
            Assert.Contains(result, m => m.Name == "Material2");
        }

        [Fact]
        public async Task GetMaterialByIdAsync_ReturnsMaterial_WhenMaterialExists()
        {
            // Arrange
            var materials = new List<Material>
            {
                new Material { Id = 1, Name = "Material1", MaterialGroupId = 1, Machinable = true, Printable = false }
            };

            _mockContext.Setup(c => c.Material).Returns(GetMockDbSet(materials).Object);
            _mockContext.Setup(c => c.Material.FindAsync(1)).ReturnsAsync(materials.FirstOrDefault(m => m.Id == 1)!);

            // Act
            var result = await _service.GetMaterialByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Material1", result.Name);
        }

        [Fact]
        public async Task GetMaterialByIdAsync_ReturnsNull_WhenMaterialDoesNotExist()
        {
            // Arrange
            var materials = new List<Material>();

            _mockContext.Setup(c => c.Material).Returns(GetMockDbSet(materials).Object);
            _mockContext.Setup(c => c.Material.FindAsync(It.IsAny<int>())).ReturnsAsync((Material)null!);

            // Act
            var result = await _service.GetMaterialByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateMaterialAsync_CreatesNewMaterial()
        {
            // Arrange
            var request = new CreateMaterialRequest { Name = "NewMaterial", MaterialGroupId = 1, Machinable = true, Printable = true };
            var materials = new List<Material>();
            var mockSet = GetMockDbSet(materials);
            _mockContext.Setup(c => c.Material).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.CreateMaterialAsync(request);

            // Assert
            mockSet.Verify(m => m.Add(It.IsAny<Material>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("NewMaterial", result.Name);
            Assert.Single(materials);
        }

        [Fact]
        public async Task DeleteMaterialAsync_DeletesMaterial_WhenMaterialExists()
        {
            // Arrange
            var existingMaterial = new Material { Id = 1, Name = "DeleteMaterial", MaterialGroupId = 1, Machinable = true, Printable = false };
            var materials = new List<Material> { existingMaterial };
            var mockSet = GetMockDbSet(materials);
            _mockContext.Setup(c => c.Material).Returns(mockSet.Object);
            _mockContext.Setup(c => c.Material.FindAsync(1)).ReturnsAsync(existingMaterial);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteMaterialAsync(1);

            // Assert
            mockSet.Verify(m => m.Remove(existingMaterial), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.True(result);
            Assert.Empty(materials);
        }

        [Fact]
        public async Task DeleteMaterialAsync_ReturnsFalse_WhenMaterialDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(c => c.Material.FindAsync(It.IsAny<int>())).ReturnsAsync((Material)null!); // Added !

            // Act
            var result = await _service.DeleteMaterialAsync(1);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
            Assert.False(result);
        }
    }
}