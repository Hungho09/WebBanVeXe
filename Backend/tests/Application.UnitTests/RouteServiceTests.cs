using Application.DTOs.Route;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace Application.UnitTests
{
    public class RouteServiceTests
    {
        private readonly Mock<IRouteRepository> _routeRepositoryMock;
        private readonly RouteService _routeService;

        public RouteServiceTests()
        {
            _routeRepositoryMock = new Mock<IRouteRepository>();
            _routeService = new RouteService(_routeRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateRouteAsync_ShouldReturnRouteDto_WhenValid()
        {
            // Arrange
            var dto = new CreateRouteDto 
            { 
                Origin = "Hanoi", 
                Destination = "HCM", 
                DistanceKm = 1700,
                Points = "Da Nang"
            };

            _routeRepositoryMock.Setup(r => r.ExistsAsync(dto.Origin, dto.Destination))
                .ReturnsAsync(false);
            
            _routeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Route>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _routeService.CreateRouteAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Origin, result.Origin);
            Assert.Equal(dto.Destination, result.Destination);
            _routeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Route>()), Times.Once);
            _routeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateRouteAsync_ShouldThrowException_WhenRouteExists()
        {
            // Arrange
            var dto = new CreateRouteDto { Origin = "Hanoi", Destination = "HCM", DistanceKm = 1700 };
            _routeRepositoryMock.Setup(r => r.ExistsAsync(dto.Origin, dto.Destination))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _routeService.CreateRouteAsync(dto));
        }

        [Fact]
        public async Task GetRouteByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _routeRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Route?)null);

            // Act
            var result = await _routeService.GetRouteByIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteRouteAsync_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var route = new Route { Id = id };
            _routeRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(route);

            // Act
            var result = await _routeService.DeleteRouteAsync(id);

            // Assert
            Assert.True(result);
            _routeRepositoryMock.Verify(r => r.Delete(route), Times.Once);
            _routeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
