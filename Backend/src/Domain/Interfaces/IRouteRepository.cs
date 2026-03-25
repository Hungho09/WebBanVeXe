using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRouteRepository
    {
        Task<Route?> GetByIdAsync(Guid id);
        Task<IEnumerable<Route>> GetAllAsync();
        Task AddAsync(Route route);
        void Update(Route route);
        void Delete(Route route);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(string origin, string destination);
        Task<(IEnumerable<string> Origins, IEnumerable<string> Destinations)> GetDistinctLocationsAsync();
    }
}
