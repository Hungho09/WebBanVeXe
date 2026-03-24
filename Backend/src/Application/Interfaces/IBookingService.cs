using System;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBookingService
    {
        Task<bool> LockSeatAsync(Guid seatId);
        Task<bool> UnlockSeatAsync(Guid seatId);
        Task<Guid> CreateBookingAsync(string userName, Guid tripId, Guid[] seatIds);
    }
}
