using Application.DTOs.Trip;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Services;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UnitTests
{
    public class TripServiceTests
    {
        private readonly Mock<ITripRepository> _tripRepositoryMock;
        private readonly TripService _tripService;

        public TripServiceTests()
        {
            _tripRepositoryMock = new Mock<ITripRepository>();
            _tripService = new TripService(_tripRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateTripAsync_ShouldReturnTripDto_WhenValid()
        {
            // Arrange
            var dto = new CreateTripDto
            {
                RouteId = Guid.NewGuid(),
                BusId = Guid.NewGuid(),
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(5),
                Price = 250000
            };

            _tripRepositoryMock.Setup(r => r.HasConflictAsync(dto.BusId, dto.DepartureTime, dto.ArrivalTime, null))
                .ReturnsAsync(false);
            
            _tripRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Trip>()))
                .Returns(Task.CompletedTask);

            // Mock GetById to simulate refreshing navigation properties
            var trip = new Trip { Id = Guid.NewGuid(), RouteId = dto.RouteId, BusId = dto.BusId };
            _tripRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(trip);

            // Act
            var result = await _tripService.CreateTripAsync(dto);

            // Assert
            Assert.NotNull(result);
            _tripRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Trip>()), Times.Once);
            _tripRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateTripAsync_ShouldThrowException_WhenTimeInvalid()
        {
            // Arrange
            var dto = new CreateTripDto
            {
                DepartureTime = DateTime.UtcNow.AddHours(5),
                ArrivalTime = DateTime.UtcNow.AddHours(2) // Invalid: arrival before departure
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _tripService.CreateTripAsync(dto));
        }

        [Fact]
        public async Task CreateTripAsync_ShouldThrowException_WhenBusHasConflict()
        {
            // Arrange
            var dto = new CreateTripDto
            {
                BusId = Guid.NewGuid(),
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(5)
            };

            _tripRepositoryMock.Setup(r => r.HasConflictAsync(dto.BusId, dto.DepartureTime, dto.ArrivalTime, null))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _tripService.CreateTripAsync(dto));
        }

        [Fact]
        public async Task GetTripByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _tripRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Trip?)null);

            // Act
            var result = await _tripService.GetTripByIdAsync(id);

            // Assert
            Assert.Null(result);
        }
    }
}
