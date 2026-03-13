using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IBusService
    {
        Task<IEnumerable<BusDto>> GetAllBusesAsync();
        Task<BusDto?> GetBusByIdAsync(Guid id);
        Task<BusDto> CreateBusAsync(CreateBusDto createBusDto);
        Task<bool> UpdateBusAsync(Guid id, UpdateBusDto updateBusDto);
        Task<bool> DeleteBusAsync(Guid id);
    }
}
