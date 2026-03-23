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

        public TripService(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
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
