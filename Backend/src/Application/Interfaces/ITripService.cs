using Application.DTOs.Trip;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITripService
    {
        Task<TripDto?> GetTripByIdAsync(Guid id);
        Task<IEnumerable<TripDto>> GetAllTripsAsync();
        Task<TripDto> CreateTripAsync(CreateTripDto createTripDto);
        Task<bool> UpdateTripAsync(Guid id, UpdateTripDto updateTripDto);
        Task<bool> DeleteTripAsync(Guid id);
    }
}
