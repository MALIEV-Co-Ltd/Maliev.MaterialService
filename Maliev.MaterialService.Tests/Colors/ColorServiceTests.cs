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

namespace Maliev.MaterialService.Tests.Colors
{
    public class ColorServiceTests
    {
        private readonly MaterialServiceService _service;
        private readonly Mock<MaterialContext> _mockContext;
        private readonly Mock<ILogger<MaterialServiceService>> _mockLogger;

        public ColorServiceTests()
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
        public async Task GetAllColorsAsync_ReturnsAllColors()
        {
            // Arrange
            var colors = new List<Color>
            {
                new Color { Id = 1, Name = "Red" },
                new Color { Id = 2, Name = "Blue" }
            };

            _mockContext.Setup(c => c.Color).Returns(GetMockDbSet(colors).Object);

            // Act
            var result = await _service.GetAllColorsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Red");
            Assert.Contains(result, c => c.Name == "Blue");
        }

        [Fact]
        public async Task GetColorByIdAsync_ReturnsColor_WhenColorExists()
        {
            // Arrange
            var colors = new List<Color>
            {
                new Color { Id = 1, Name = "Red" }
            };

            _mockContext.Setup(c => c.Color).Returns(GetMockDbSet(colors).Object);
            _mockContext.Setup(c => c.Color.FindAsync(1)).ReturnsAsync(colors.FirstOrDefault(c => c.Id == 1)!);

            // Act
            var result = await _service.GetColorByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Red", result.Name);
        }

        [Fact]
        public async Task GetColorByIdAsync_ReturnsNull_WhenColorDoesNotExist()
        {
            // Arrange
            var colors = new List<Color>();

            _mockContext.Setup(c => c.Color).Returns(GetMockDbSet(colors).Object);
            _mockContext.Setup(c => c.Color.FindAsync(It.IsAny<int>())).ReturnsAsync((Color)null!);

            // Act
            var result = await _service.GetColorByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateColorAsync_CreatesNewColor()
        {
            // Arrange
            var request = new CreateColorRequest { Name = "Green" };
            var colors = new List<Color>();
            var mockSet = GetMockDbSet(colors);
            _mockContext.Setup(c => c.Color).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.CreateColorAsync(request);

            // Assert
            mockSet.Verify(m => m.Add(It.IsAny<Color>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("Green", result.Name);
            Assert.Single(colors);
        }

        [Fact]
        public async Task UpdateColorAsync_UpdatesExistingColor_WhenColorExists()
        {
            // Arrange
            var existingColor = new Color { Id = 1, Name = "Yellow" };
            var colors = new List<Color> { existingColor };
            var mockSet = GetMockDbSet(colors);
            _mockContext.Setup(c => c.Color).Returns(mockSet.Object);
            _mockContext.Setup(c => c.Color.FindAsync(1)).ReturnsAsync(existingColor);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var request = new UpdateColorRequest { Id = 1, Name = "Orange" };

            // Act
            var result = await _service.UpdateColorAsync(request);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("Orange", result.Name);
            Assert.Equal("Orange", existingColor.Name);
        }

        [Fact]
        public async Task UpdateColorAsync_ReturnsNull_WhenColorDoesNotExist()
        {
            // Arrange
            var request = new UpdateColorRequest { Id = 1, Name = "Orange" };
            _mockContext.Setup(c => c.Color.FindAsync(It.IsAny<int>())).ReturnsAsync((Color)null!); // Added !

            // Act
            var result = await _service.UpdateColorAsync(request);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteColorAsync_DeletesColor_WhenColorExists()
        {
            // Arrange
            var existingColor = new Color { Id = 1, Name = "Purple" };
            var colors = new List<Color> { existingColor };
            var mockSet = GetMockDbSet(colors);
            _mockContext.Setup(c => c.Color).Returns(mockSet.Object);
            _mockContext.Setup(c => c.Color.FindAsync(1)).ReturnsAsync(existingColor);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteColorAsync(1);

            // Assert
            mockSet.Verify(m => m.Remove(existingColor), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.True(result);
            Assert.Empty(colors);
        }

        [Fact]
        public async Task DeleteColorAsync_ReturnsFalse_WhenColorDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(c => c.Color.FindAsync(It.IsAny<int>())).ReturnsAsync((Color)null!); // Added !

            // Act
            var result = await _service.DeleteColorAsync(1);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
            Assert.False(result);
        }
    }
}