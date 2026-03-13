using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBusRepository
    {
        Task<IEnumerable<Bus>> GetAllAsync();
        Task<Bus?> GetByIdAsync(Guid id);
        Task<bool> ExistsByPlateNumberAsync(string plateNumber);
        Task<Bus> AddAsync(Bus bus);
        Task UpdateAsync(Bus bus);
        Task DeleteAsync(Bus bus);
    }
}
