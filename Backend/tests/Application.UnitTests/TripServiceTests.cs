using Application.DTOs.Trip;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
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
        private readonly Mock<ISeatService> _seatServiceMock;
        private readonly Mock<IBusRepository> _busRepositoryMock;
        private readonly TripService _tripService;

        public TripServiceTests()
        {
            _tripRepositoryMock = new Mock<ITripRepository>();
            _seatServiceMock = new Mock<ISeatService>();
            _busRepositoryMock = new Mock<IBusRepository>();
            _tripService = new TripService(_tripRepositoryMock.Object, _seatServiceMock.Object, _busRepositoryMock.Object);
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

            _busRepositoryMock.Setup(b => b.GetByIdAsync(dto.BusId))
                .ReturnsAsync(new Bus { Id = dto.BusId, Status = BusStatus.Available });
            _busRepositoryMock.Setup(b => b.UpdateAsync(It.IsAny<Bus>())).Returns(Task.CompletedTask);

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
            _busRepositoryMock.Verify(b => b.UpdateAsync(It.IsAny<Bus>()), Times.Once);
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

            _busRepositoryMock.Setup(b => b.GetByIdAsync(dto.BusId))
                .ReturnsAsync(new Bus { Id = dto.BusId, Status = BusStatus.Available });

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
