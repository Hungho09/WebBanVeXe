using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly ApplicationDbContext _context;

        public RouteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Route?> GetByIdAsync(Guid id)
        {
            return await _context.Routes.FindAsync(id);
        }

        public async Task<IEnumerable<Route>> GetAllAsync()
        {
            return await _context.Routes
                .OrderBy(r => r.Origin)
                .ThenBy(r => r.Destination)
                .ToListAsync();
        }

        public async Task AddAsync(Route route)
        {
            await _context.Routes.AddAsync(route);
        }

        public void Update(Route route)
        {
            _context.Routes.Update(route);
        }

        public void Delete(Route route)
        {
            _context.Routes.Remove(route);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string origin, string destination)
        {
            return await _context.Routes
                .AnyAsync(r => r.Origin.ToLower() == origin.ToLower() 
                    && r.Destination.ToLower() == destination.ToLower());
        }
    }
}
