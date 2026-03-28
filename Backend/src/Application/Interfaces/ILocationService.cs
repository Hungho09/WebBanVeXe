using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Location;

namespace Application.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsByProvinceAsync(string? searchTerm = null);
        Task<LocationDto?> GetLocationByIdAsync(Guid id);
        Task<LocationDto> CreateLocationAsync(CreateLocationDto createDto);
        Task<bool> UpdateLocationAsync(Guid id, UpdateLocationDto updateDto);
        Task<bool> DeleteLocationAsync(Guid id);
    }
}
