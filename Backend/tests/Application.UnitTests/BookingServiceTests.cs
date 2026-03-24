using Application.DTOs.Booking;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;

namespace Application.UnitTests
{
    public class BookingServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly BookingService _service;
        private readonly Mock<ITripService> _tripServiceMock = new();

        public BookingServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            var repository = new BookingRepository(_context);
            _service = new BookingService(_context, repository, _tripServiceMock.Object);
        }

        public void Dispose() => _context.Dispose();

        private async Task<(Guid userId, Guid tripId, Guid seatA, Guid seatB)> SeedTripWithTwoAvailableSeatsAsync()
        {
            var busTypeId = Guid.NewGuid();
            _context.BusTypes.Add(new BusType
            {
                Id = busTypeId,
                Name = "Ghế ngồi",
                SeatCount = 40
            });

            var busId = Guid.NewGuid();
            _context.Buses.Add(new Bus
            {
                Id = busId,
                BusTypeId = busTypeId,
                PlateNumber = "51B-000.00",
                CompanyName = "Test",
                SeatCount = 40,
                Status = BusStatus.Available
            });

            var routeId = Guid.NewGuid();
            _context.Routes.Add(new Route
            {
                Id = routeId,
                Origin = "A",
                Destination = "B",
                DistanceKm = 100
            });

            var tripId = Guid.NewGuid();
            _context.Trips.Add(new Trip
            {
                Id = tripId,
                RouteId = routeId,
                BusId = busId,
                DepartureTime = DateTime.UtcNow.AddDays(1),
                ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(5),
                Price = 150_000m
            });

            var userId = Guid.NewGuid();
            _context.Users.Add(new User
            {
                Id = userId,
                UserName = "customer1",
                Email = "c1@test.local",
                PasswordHash = "x",
                FullName = "C1",
                PhoneNumber = "0900000001",
                Role = "Customer"
            });

            var seatA = Guid.NewGuid();
            var seatB = Guid.NewGuid();
            _context.Seats.Add(new Seat
            {
                Id = seatA,
                TripId = tripId,
                SeatNumber = "A1",
                RowNumber = 1,
                ColumnNumber = 1,
                Floor = 1,
                Status = SeatStatus.Available
            });
            _context.Seats.Add(new Seat
            {
                Id = seatB,
                TripId = tripId,
                SeatNumber = "A2",
                RowNumber = 1,
                ColumnNumber = 2,
                Floor = 1,
                Status = SeatStatus.Available
            });

            await _context.SaveChangesAsync();
            return (userId, tripId, seatA, seatB);
        }

        [Fact]
        public async Task CreateBookingAsync_ReturnsPersistedDetailIds_AndMarksSeatsBooked()
        {
            var (userId, tripId, seatA, seatB) = await SeedTripWithTwoAvailableSeatsAsync();

            var dto = new CreateBookingDto
            {
                UserId = userId,
                TripId = tripId,
                SeatIds = new List<Guid> { seatA, seatB }
            };

            var response = await _service.CreateBookingAsync(dto);

            Assert.NotEqual(Guid.Empty, response.Id);
            Assert.Equal(2, response.Details.Count);
            Assert.All(response.Details, d => Assert.NotEqual(Guid.Empty, d.Id));
            Assert.Contains(response.Details, d => d.SeatId == seatA && d.SeatNumber == "A1");
            Assert.Equal(300_000m, response.TotalAmount);

            var seats = await _context.Seats.Where(s => s.Id == seatA || s.Id == seatB).ToListAsync();
            Assert.All(seats, s => Assert.Equal(SeatStatus.Booked, s.Status));

            var storedDetails = await _context.BookingDetails.Where(d => d.BookingId == response.Id).ToListAsync();
            Assert.Equal(2, storedDetails.Count);
            Assert.Equal(response.Details.Select(x => x.Id).OrderBy(x => x), storedDetails.Select(x => x.Id).OrderBy(x => x));
        }

        [Fact]
        public async Task CreateBookingAsync_AllowsLockedSeats_WhenSameUserHoldsLock()
        {
            var (userId, tripId, seatA, _) = await SeedTripWithTwoAvailableSeatsAsync();
            var seat = await _context.Seats.FindAsync(seatA);
            seat!.Status = SeatStatus.Locked;
            seat.LockedByUserId = userId;
            seat.LockExpirationTime = DateTime.UtcNow.AddMinutes(5);
            await _context.SaveChangesAsync();

            var response = await _service.CreateBookingAsync(new CreateBookingDto
            {
                UserId = userId,
                TripId = tripId,
                SeatIds = new List<Guid> { seatA }
            });

            Assert.Single(response.Details);
            Assert.Equal(SeatStatus.Booked, (await _context.Seats.FindAsync(seatA))!.Status);
        }

        [Fact]
        public async Task CreateBookingAsync_Throws_WhenLockedByOtherUser()
        {
            var (userId, tripId, seatA, _) = await SeedTripWithTwoAvailableSeatsAsync();
            var other = Guid.NewGuid();
            _context.Users.Add(new User
            {
                Id = other,
                UserName = "other",
                Email = "o@test.local",
                PasswordHash = "x",
                FullName = "O",
                PhoneNumber = "0900000002",
                Role = "Customer"
            });
            var seat = await _context.Seats.FindAsync(seatA);
            seat!.Status = SeatStatus.Locked;
            seat.LockedByUserId = other;
            seat.LockExpirationTime = DateTime.UtcNow.AddMinutes(5);
            await _context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateBookingAsync(new CreateBookingDto
                {
                    UserId = userId,
                    TripId = tripId,
                    SeatIds = new List<Guid> { seatA }
                }));
        }

        [Fact]
        public async Task CreateBookingAsync_Throws_WhenLockExpired()
        {
            var (userId, tripId, seatA, _) = await SeedTripWithTwoAvailableSeatsAsync();
            var seat = await _context.Seats.FindAsync(seatA);
            seat!.Status = SeatStatus.Locked;
            seat.LockedByUserId = userId;
            seat.LockExpirationTime = DateTime.UtcNow.AddMinutes(-1);
            await _context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateBookingAsync(new CreateBookingDto
                {
                    UserId = userId,
                    TripId = tripId,
                    SeatIds = new List<Guid> { seatA }
                }));
        }

        [Fact]
        public async Task CreateBookingAsync_SecondUserCannotBookSameSeat()
        {
            var (userId, tripId, seatA, _) = await SeedTripWithTwoAvailableSeatsAsync();
            var other = Guid.NewGuid();
            _context.Users.Add(new User
            {
                Id = other,
                UserName = "other2",
                Email = "o2@test.local",
                PasswordHash = "x",
                FullName = "O2",
                PhoneNumber = "0900000003",
                Role = "Customer"
            });
            await _context.SaveChangesAsync();

            await _service.CreateBookingAsync(new CreateBookingDto
            {
                UserId = userId,
                TripId = tripId,
                SeatIds = new List<Guid> { seatA }
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateBookingAsync(new CreateBookingDto
                {
                    UserId = other,
                    TripId = tripId,
                    SeatIds = new List<Guid> { seatA }
                }));
        }

        [Fact]
        public async Task CreateBookingAsync_Throws_WhenSeatNotOnTrip()
        {
            var (userId, tripId, seatA, _) = await SeedTripWithTwoAvailableSeatsAsync();

            var otherTripId = Guid.NewGuid();
            _context.Trips.Add(new Trip
            {
                Id = otherTripId,
                RouteId = (await _context.Routes.FirstAsync()).Id,
                BusId = (await _context.Buses.FirstAsync()).Id,
                DepartureTime = DateTime.UtcNow.AddDays(2),
                ArrivalTime = DateTime.UtcNow.AddDays(2).AddHours(3),
                Price = 50_000m
            });
            var alienSeat = Guid.NewGuid();
            _context.Seats.Add(new Seat
            {
                Id = alienSeat,
                TripId = otherTripId,
                SeatNumber = "X1",
                RowNumber = 1,
                ColumnNumber = 1,
                Floor = 1,
                Status = SeatStatus.Available
            });
            await _context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateBookingAsync(new CreateBookingDto
                {
                    UserId = userId,
                    TripId = tripId,
                    SeatIds = new List<Guid> { alienSeat }
                }));
        }

        [Fact]
        public async Task CreateBookingAsync_Throws_WhenDuplicateSeatIds()
        {
            var (userId, tripId, seatA, _) = await SeedTripWithTwoAvailableSeatsAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateBookingAsync(new CreateBookingDto
                {
                    UserId = userId,
                    TripId = tripId,
                    SeatIds = new List<Guid> { seatA, seatA }
                }));
        }

        [Fact]
        public async Task CreateBookingAsync_Throws_WhenSeatAlreadyBooked()
        {
            var (userId, tripId, seatA, _) = await SeedTripWithTwoAvailableSeatsAsync();
            var seat = await _context.Seats.FindAsync(seatA);
            seat!.Status = SeatStatus.Booked;
            await _context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateBookingAsync(new CreateBookingDto
                {
                    UserId = userId,
                    TripId = tripId,
                    SeatIds = new List<Guid> { seatA }
                }));
        }

        [Fact]
        public async Task GetBookingByIdAsync_ReturnsDto_WhenExists()
        {
            var (userId, tripId, seatA, _) = await SeedTripWithTwoAvailableSeatsAsync();
            var created = await _service.CreateBookingAsync(new CreateBookingDto
            {
                UserId = userId,
                TripId = tripId,
                SeatIds = new List<Guid> { seatA }
            });

            var fetched = await _service.GetBookingByIdAsync(created.Id);

            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched!.Id);
            Assert.Equal("customer1", fetched.UserName);
            Assert.Single(fetched.Details);
        }

        [Fact]
        public async Task CancelBookingAsync_ReleasesSeats()
        {
            var (userId, tripId, seatA, _) = await SeedTripWithTwoAvailableSeatsAsync();
            var created = await _service.CreateBookingAsync(new CreateBookingDto
            {
                UserId = userId,
                TripId = tripId,
                SeatIds = new List<Guid> { seatA }
            });

            var ok = await _service.CancelBookingAsync(created.Id);
            Assert.True(ok);

            var seat = await _context.Seats.FindAsync(seatA);
            Assert.Equal(SeatStatus.Available, seat!.Status);
            var booking = await _context.Bookings.FindAsync(created.Id);
            Assert.Equal(BookingStatus.Cancelled, booking!.BookingStatus);
        }
    }
}
