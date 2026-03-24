using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(Guid id);
        Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task<bool> SaveChangesAsync();
    }
}
