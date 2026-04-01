using Application.DTOs.Trip;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly ISeatService _seatService;
        private readonly IBusRepository _busRepository;

        // ✅ Bỏ IApplicationDbContext vì không còn dùng _context.Locations nữa
        public TripService(ITripRepository tripRepository, ISeatService seatService, IBusRepository busRepository)
        {
            _tripRepository = tripRepository;
            _seatService = seatService;
            _busRepository = busRepository;
        }

        public async Task<TripDto?> GetTripByIdAsync(Guid id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null) return null;
            return MapToDto(trip);
        }

        public async Task<IEnumerable<TripDto>> GetAllTripsAsync()
        {
            var trips = await _tripRepository.GetAllAsync();
            return trips.Select(MapToDto);
        }

        public async Task<TripDto> CreateTripAsync(CreateTripDto dto)
        {
            if (dto.ArrivalTime <= dto.DepartureTime)
                throw new ArgumentException("Arrival time must be after departure time.");

            var bus = await _busRepository.GetByIdAsync(dto.BusId);
            if (bus == null)
                throw new ArgumentException("Bus not found.");

            if (bus.Status != Domain.Enums.BusStatus.Available)
            {
                var statusMsg = bus.Status == Domain.Enums.BusStatus.Active
                    ? "đang hoạt động" : "ngưng hoạt động";
                throw new InvalidOperationException(
                    $"Không thể tạo chuyến mới cho xe này vì xe {statusMsg}.");
            }

            if (await _tripRepository.HasConflictAsync(dto.BusId, dto.DepartureTime, dto.ArrivalTime))
                throw new InvalidOperationException(
                    "Xe này đã được xếp lịch cho một chuyến đi khác trong khoảng thời gian này.");

            var trip = new Trip
            {
                Id = Guid.NewGuid(),
                RouteId = dto.RouteId,
                BusId = dto.BusId,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                Price = dto.Price,
                Status = Domain.Enums.TripStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _tripRepository.AddAsync(trip);

            bus.Status = Domain.Enums.BusStatus.Active;
            await _busRepository.UpdateAsync(bus);
            await _tripRepository.SaveChangesAsync();

            var createdTripWithBus = await _tripRepository.GetByIdAsync(trip.Id);
            if (createdTripWithBus?.Bus != null)
                await _seatService.GenerateSeatsForTripAsync(trip.Id, createdTripWithBus.Bus.BusTypeId);

            var createdTrip = await _tripRepository.GetByIdAsync(trip.Id);
            return MapToDto(createdTrip!);
        }

        public async Task<bool> UpdateTripAsync(Guid id, UpdateTripDto dto)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null) return false;

            if (dto.ArrivalTime <= dto.DepartureTime)
                throw new ArgumentException("Arrival time must be after departure time.");

            if (await _tripRepository.HasConflictAsync(dto.BusId, dto.DepartureTime, dto.ArrivalTime, id))
                throw new InvalidOperationException(
                    "This bus is already assigned to another trip during this interval.");

            trip.RouteId = dto.RouteId;
            trip.BusId = dto.BusId;
            trip.DepartureTime = dto.DepartureTime;
            trip.ArrivalTime = dto.ArrivalTime;
            trip.Price = dto.Price;
            trip.Status = dto.Status;
            trip.UpdatedAt = DateTime.UtcNow;

            _tripRepository.Update(trip);
            await _tripRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTripAsync(Guid id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null) return false;

            _tripRepository.Delete(trip);
            await _tripRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SeatDto>> GetSeatsByTripIdAsync(Guid tripId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null) return Enumerable.Empty<SeatDto>();

            var now = DateTime.UtcNow;

            return trip.Seats.Select(s => new SeatDto
            {
                Id = s.Id,
                TripId = s.TripId,
                SeatNumber = s.SeatNumber,
                Status = (s.Status == Domain.Enums.SeatStatus.Locked && s.LockExpirationTime.HasValue && s.LockExpirationTime <= now) 
                         ? Domain.Enums.SeatStatus.Available.ToString() 
                         : s.Status.ToString(),
                RowNumber = s.RowNumber,
                ColumnNumber = s.ColumnNumber,
                Floor = s.Floor,
                Type = s.Type.ToString(),
                LockExpirationTime = s.LockExpirationTime,
                LockedByUserId = s.LockedByUserId
            });
        }

        public async Task<bool> LockSeatAsync(Guid seatId, Guid userId)
        {
            if (userId == Guid.Empty) return false;

            var trip = await _tripRepository.GetBySeatIdAsync(seatId);
            if (trip == null) return false;

            var seat = trip.Seats.FirstOrDefault(s => s.Id == seatId);
            if (seat == null) return false;

            var now = DateTime.UtcNow;

            if (seat.Status == Domain.Enums.SeatStatus.Booked)
                return false;

            if (seat.Status == Domain.Enums.SeatStatus.Locked
                && seat.LockExpirationTime > now
                && seat.LockedByUserId != userId)
                return false;

            if (seat.Status == Domain.Enums.SeatStatus.Available)
            {
                seat.Status = Domain.Enums.SeatStatus.Locked;
                seat.LockExpirationTime = now.AddMinutes(10);
                seat.LockedByUserId = userId;
                _tripRepository.Update(trip);
                await _tripRepository.SaveChangesAsync();
                return true;
            }

            if (seat.Status == Domain.Enums.SeatStatus.Locked && seat.LockExpirationTime > now)
            {
                if (seat.LockedByUserId == userId)
                {
                    seat.LockExpirationTime = now.AddMinutes(10);
                    _tripRepository.Update(trip);
                    await _tripRepository.SaveChangesAsync();
                    return true;
                }
                return false;
            }

            return false;
        }

        public async Task<bool> UnlockSeatAsync(Guid seatId, Guid userId)
        {
            if (userId == Guid.Empty) return false;

            var trip = await _tripRepository.GetBySeatIdAsync(seatId);
            if (trip == null) return false;

            var seat = trip.Seats.FirstOrDefault(s => s.Id == seatId);
            if (seat == null
                || seat.Status != Domain.Enums.SeatStatus.Locked
                || (seat.LockedByUserId != null && seat.LockedByUserId != userId))
                return false;

            var now = DateTime.UtcNow;

            if (seat.LockExpirationTime.HasValue && seat.LockExpirationTime <= now)
            {
                seat.Status = Domain.Enums.SeatStatus.Available;
                seat.LockExpirationTime = null;
                seat.LockedByUserId = null;
                _tripRepository.Update(trip);
                await _tripRepository.SaveChangesAsync();
                return true;
            }

            if (seat.LockedByUserId != userId) return false;

            seat.Status = Domain.Enums.SeatStatus.Available;
            seat.LockedByUserId = null;
            seat.LockExpirationTime = null;
            _tripRepository.Update(trip);
            await _tripRepository.SaveChangesAsync();
            return true;
        }

        // ✅ SỬA: Lấy từ RouteStops → Location thay vì _context.Locations cũ
        public async Task<IEnumerable<TripPointDto>> GetTripPointsAsync(Guid tripId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null || trip.Route == null)
                return Enumerable.Empty<TripPointDto>();

            if (trip.Route.RouteStops == null || !trip.Route.RouteStops.Any())
                return Enumerable.Empty<TripPointDto>();

            var result = new List<TripPointDto>();

            foreach (var rs in trip.Route.RouteStops.OrderBy(rs => rs.OrderIndex))
            {
                var loc = rs.Location; // ✅ dùng tên cũ Location
                if (loc == null) continue;

                result.Add(new TripPointDto
                {
                    Id = loc.Id,
                    Name = loc.Name,
                    Address = loc.Address,
                    ExpectedTime = trip.DepartureTime.AddMinutes(rs.OffsetMinutes),
                    DistanceFromOrigin = rs.DistanceFromOriginKm,
                    IsPickup = loc.IsPickup,
                    IsDropoff = loc.IsDropoff,
                    Badge = loc.Badge
                });
            }

            return result;
        }

        public async Task<IEnumerable<TripDto>> SearchTripsAsync(
            string? origin, string? destination, DateTime? date)
        {
            var trips = await _tripRepository.SearchAsync(origin, destination, date);
            return trips.Select(MapToDto);
        }

        // ✅ SỬA: Thêm RouteStops vào response, dùng tên Location
        private TripDto MapToDto(Trip trip)
        {
            var availableSeats = 0;
            if (trip.Seats != null && trip.Seats.Any())
            {
                var now = DateTime.UtcNow;
                availableSeats = trip.Seats.Count(s =>
                    s.Status == Domain.Enums.SeatStatus.Available ||
                    (s.Status == Domain.Enums.SeatStatus.Locked
                        && s.LockExpirationTime.HasValue
                        && s.LockExpirationTime <= now));
            }

            return new TripDto
            {
                Id = trip.Id,
                RouteId = trip.RouteId,
                RouteName = trip.Route != null
                    ? $"{trip.Route.Origin} - {trip.Route.Destination}"
                    : "N/A",
                BusId = trip.BusId,
                BusPlate = trip.Bus?.PlateNumber ?? "N/A",
                BusTypeName = trip.Bus?.BusType?.Name ?? "N/A",
                BusImageUrl = trip.Bus?.ImageUrl,
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                Price = trip.Price,
                Status = trip.Status.ToString(),
                AvailableSeats = availableSeats,
                CreatedAt = trip.CreatedAt,
                UpdatedAt = trip.UpdatedAt,

                // ✅ Thêm mới: map RouteStops dùng tên Location
                RouteStops = trip.Route?.RouteStops?
                    .OrderBy(rs => rs.OrderIndex)
                    .Select(rs => new RouteStopDto
                    {
                        Id = rs.Id,
                        OrderIndex = rs.OrderIndex,
                        OffsetMinutes = rs.OffsetMinutes,
                        DistanceFromOriginKm = rs.DistanceFromOriginKm,
                        StopPointId = rs.Location?.Id ?? Guid.Empty,
                        StopPointName = rs.Location?.Name,
                        StopPointAddress = rs.Location?.Address,
                        Badge = rs.Location?.Badge,
                        IsPickup = rs.Location?.IsPickup ?? false,
                        IsDropoff = rs.Location?.IsDropoff ?? false,
                        IsDefault = rs.Location?.IsDefault ?? false,
                        ExpectedTime = trip.DepartureTime.AddMinutes(rs.OffsetMinutes)
                    }).ToList() ?? new List<RouteStopDto>()
            };
        }
    }
}