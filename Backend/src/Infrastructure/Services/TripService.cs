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

        public TripService(ITripRepository tripRepository, ISeatService seatService)
        {
            _tripRepository = tripRepository;
            _seatService = seatService;
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
            // BVX-66: Validate Trip time logic
            if (dto.ArrivalTime <= dto.DepartureTime)
            {
                throw new ArgumentException("Arrival time must be after departure time.");
            }

            // Check for bus availability (no overlapping trips)
            if (await _tripRepository.HasConflictAsync(dto.BusId, dto.DepartureTime, dto.ArrivalTime))
            {
                throw new InvalidOperationException("This bus is already assigned to another trip during this interval.");
            }

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
            await _tripRepository.SaveChangesAsync();

            // Refresh to get navigation properties (like Bus) for seat generation
            var createdTripWithBus = await _tripRepository.GetByIdAsync(trip.Id);
            if (createdTripWithBus?.Bus != null)
            {
                // Story 2.4: Auto generate seats based on bus type
                await _seatService.GenerateSeatsForTripAsync(trip.Id, createdTripWithBus.Bus.BusType);
            }

            // Refresh to get navigation properties for the DTO
            var createdTrip = await _tripRepository.GetByIdAsync(trip.Id);
            return MapToDto(createdTrip!);
        }

        public async Task<bool> UpdateTripAsync(Guid id, UpdateTripDto dto)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null) return false;

            if (dto.ArrivalTime <= dto.DepartureTime)
            {
                throw new ArgumentException("Arrival time must be after departure time.");
            }

            if (await _tripRepository.HasConflictAsync(dto.BusId, dto.DepartureTime, dto.ArrivalTime, id))
            {
                throw new InvalidOperationException("This bus is already assigned to another trip during this interval.");
            }

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

            return trip.Seats.Select(s => new SeatDto
            {
                Id = s.Id,
                TripId = s.TripId,
                SeatNumber = s.SeatNumber,
                Status = s.Status.ToString(),
                RowNumber = s.RowNumber,
                ColumnNumber = s.ColumnNumber,
                Floor = s.Floor
            });
        }

        public async Task<bool> LockSeatAsync(Guid seatId)
        {
            // Note: Since TripService normally uses TripRepository, we might need a dedicated SeatRepository 
            // but for simplicity and immediate implementation, we'll assume the repository can handle it 
            // or we'll use the context if we have access. 
            // Checking TripRepository implementation...
            
            var trip = await _tripRepository.GetBySeatIdAsync(seatId);
            if (trip == null) return false;

            var seat = trip.Seats.FirstOrDefault(s => s.Id == seatId);
            if (seat == null || seat.Status == Domain.Enums.SeatStatus.Booked) return false;

            // If already locked and not expired, return false
            if (seat.Status == Domain.Enums.SeatStatus.Locked && seat.LockExpirationTime > DateTime.UtcNow)
                return false;

            seat.Status = Domain.Enums.SeatStatus.Locked;
            seat.LockExpirationTime = DateTime.UtcNow.AddMinutes(10);

            _tripRepository.Update(trip);
            await _tripRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlockSeatAsync(Guid seatId)
        {
            var trip = await _tripRepository.GetBySeatIdAsync(seatId);
            if (trip == null) return false;

            var seat = trip.Seats.FirstOrDefault(s => s.Id == seatId);
            if (seat == null || seat.Status != Domain.Enums.SeatStatus.Locked) return false;

            seat.Status = Domain.Enums.SeatStatus.Available;
            seat.LockExpirationTime = null;

            _tripRepository.Update(trip);
            await _tripRepository.SaveChangesAsync();
            return true;
        }
        private TripDto MapToDto(Trip trip)
        {
            return new TripDto
            {
                Id = trip.Id,
                RouteId = trip.RouteId,
                RouteName = trip.Route != null ? $"{trip.Route.Origin} - {trip.Route.Destination}" : "N/A",
                BusId = trip.BusId,
                BusPlate = trip.Bus?.PlateNumber ?? "N/A",
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                Price = trip.Price,
                Status = trip.Status.ToString(),
                CreatedAt = trip.CreatedAt,
                UpdatedAt = trip.UpdatedAt
            };
        }
    }
}
