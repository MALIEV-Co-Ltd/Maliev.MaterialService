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

namespace Maliev.MaterialService.Tests.SurfaceFinishes
{
    public class SurfaceFinishServiceTests
    {
        private readonly MaterialServiceService _service;
        private readonly Mock<MaterialContext> _mockContext;
        private readonly Mock<ILogger<MaterialServiceService>> _mockLogger;

        public SurfaceFinishServiceTests()
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
        public async Task GetAllSurfaceFinishesAsync_ReturnsAllSurfaceFinishes()
        {
            // Arrange
            var surfaceFinishes = new List<SurfaceFinish>
            {
                new SurfaceFinish { Id = 1, Name = "Matte" },
                new SurfaceFinish { Id = 2, Name = "Glossy" }
            };

            _mockContext.Setup(c => c.SurfaceFinish).Returns(GetMockDbSet(surfaceFinishes).Object);

            // Act
            var result = await _service.GetAllSurfaceFinishesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, sf => sf.Name == "Matte");
            Assert.Contains(result, sf => sf.Name == "Glossy");
        }

        [Fact]
        public async Task GetSurfaceFinishByIdAsync_ReturnsSurfaceFinish_WhenSurfaceFinishExists()
        {
            // Arrange
            var surfaceFinishes = new List<SurfaceFinish>
            {
                new SurfaceFinish { Id = 1, Name = "Matte" }
            };

            _mockContext.Setup(c => c.SurfaceFinish).Returns(GetMockDbSet(surfaceFinishes).Object);
            _mockContext.Setup(c => c.SurfaceFinish.FindAsync(1)).ReturnsAsync(surfaceFinishes.FirstOrDefault(sf => sf.Id == 1)!);

            // Act
            var result = await _service.GetSurfaceFinishByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Matte", result.Name);
        }

        [Fact]
        public async Task GetSurfaceFinishByIdAsync_ReturnsNull_WhenSurfaceFinishDoesNotExist()
        {
            // Arrange
            var surfaceFinishes = new List<SurfaceFinish>();

            _mockContext.Setup(c => c.SurfaceFinish).Returns(GetMockDbSet(surfaceFinishes).Object);
            _mockContext.Setup(c => c.SurfaceFinish.FindAsync(It.IsAny<int>())).ReturnsAsync((SurfaceFinish)null!); // Added !

            // Act
            var result = await _service.GetSurfaceFinishByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateSurfaceFinishAsync_CreatesNewSurfaceFinish()
        {
            // Arrange
            var request = new CreateSurfaceFinishRequest { Name = "Satin" };
            var surfaceFinishes = new List<SurfaceFinish>();
            var mockSet = GetMockDbSet(surfaceFinishes);
            _mockContext.Setup(c => c.SurfaceFinish).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.CreateSurfaceFinishAsync(request);

            // Assert
            mockSet.Verify(m => m.Add(It.IsAny<SurfaceFinish>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("Satin", result.Name);
            Assert.Single(surfaceFinishes);
        }

        [Fact]
        public async Task UpdateSurfaceFinishAsync_UpdatesExistingSurfaceFinish_WhenSurfaceFinishExists()
        {
            // Arrange
            var existingSurfaceFinish = new SurfaceFinish { Id = 1, Name = "OldFinish" };
            var surfaceFinishes = new List<SurfaceFinish> { existingSurfaceFinish };
            var mockSet = GetMockDbSet(surfaceFinishes);
            _mockContext.Setup(c => c.SurfaceFinish).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SurfaceFinish.FindAsync(1)).ReturnsAsync(existingSurfaceFinish);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var request = new UpdateSurfaceFinishRequest { Id = 1, Name = "NewFinish" };

            // Act
            var result = await _service.UpdateSurfaceFinishAsync(request);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("NewFinish", result.Name);
            Assert.Equal("NewFinish", existingSurfaceFinish.Name);
        }

        [Fact]
        public async Task UpdateSurfaceFinishAsync_ReturnsNull_WhenSurfaceFinishDoesNotExist()
        {
            // Arrange
            var request = new UpdateSurfaceFinishRequest { Id = 1, Name = "NewFinish" };
            _mockContext.Setup(c => c.SurfaceFinish.FindAsync(It.IsAny<int>())).ReturnsAsync((SurfaceFinish)null!); // Added !

            // Act
            var result = await _service.UpdateSurfaceFinishAsync(request);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteSurfaceFinishAsync_DeletesSurfaceFinish_WhenSurfaceFinishExists()
        {
            // Arrange
            var existingSurfaceFinish = new SurfaceFinish { Id = 1, Name = "DeleteFinish" };
            var surfaceFinishes = new List<SurfaceFinish> { existingSurfaceFinish };
            var mockSet = GetMockDbSet(surfaceFinishes);
            _mockContext.Setup(c => c.SurfaceFinish).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SurfaceFinish.FindAsync(1)).ReturnsAsync(existingSurfaceFinish);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteSurfaceFinishAsync(1);

            // Assert
            mockSet.Verify(m => m.Remove(existingSurfaceFinish), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.True(result);
            Assert.Empty(surfaceFinishes);
        }

        [Fact]
        public async Task DeleteSurfaceFinishAsync_ReturnsFalse_WhenSurfaceFinishDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(c => c.SurfaceFinish.FindAsync(It.IsAny<int>())).ReturnsAsync((SurfaceFinish)null!); // Added !

            // Act
            var result = await _service.DeleteSurfaceFinishAsync(1);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
            Assert.False(result);
        }
    }
}