using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(Guid id);
        Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Booking>> GetPendingCancellationsAsync();
        Task<Booking> AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task<bool> SaveChangesAsync();
    }
}
