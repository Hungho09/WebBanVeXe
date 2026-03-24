using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITripRepository
    {
        Task<Trip?> GetByIdAsync(Guid id);
        Task<IEnumerable<Trip>> GetAllAsync();
        Task<IEnumerable<Trip>> GetByRouteAsync(Guid routeId);
        Task AddAsync(Trip trip);
        void Update(Trip trip);
        void Delete(Trip trip);
        Task SaveChangesAsync();
        Task<bool> HasConflictAsync(Guid busId, DateTime departureTime, DateTime arrivalTime, Guid? excludeTripId = null);
        Task<Trip?> GetBySeatIdAsync(Guid seatId);
    }
}
