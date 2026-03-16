using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            return await _context.Routes.ToListAsync();
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
    }
}
