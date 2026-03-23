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
    public class TripRepository : ITripRepository
    {
        private readonly ApplicationDbContext _context;

        public TripRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Trip?> GetByIdAsync(Guid id)
        {
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Bus)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Trip>> GetAllAsync()
        {
            return await _context.Trips
                .Include(t => t.Route)
                .Include(t => t.Bus)
                .OrderByDescending(t => t.DepartureTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trip>> GetByRouteAsync(Guid routeId)
        {
            return await _context.Trips
                .Include(t => t.Bus)
                .Where(t => t.RouteId == routeId)
                .OrderBy(t => t.DepartureTime)
                .ToListAsync();
        }

        public async Task AddAsync(Trip trip)
        {
            await _context.Trips.AddAsync(trip);
        }

        public void Update(Trip trip)
        {
            _context.Trips.Update(trip);
        }

        public void Delete(Trip trip)
        {
            _context.Trips.Remove(trip);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasConflictAsync(Guid busId, DateTime departureTime, DateTime arrivalTime, Guid? excludeTripId = null)
        {
            return await _context.Trips
                .AnyAsync(t => t.BusId == busId 
                    && t.Id != excludeTripId
                    && ((departureTime >= t.DepartureTime && departureTime < t.ArrivalTime)
                        || (arrivalTime > t.DepartureTime && arrivalTime <= t.ArrivalTime)
                        || (departureTime <= t.DepartureTime && arrivalTime >= t.ArrivalTime)));
        }

        public async Task<Trip?> GetBySeatIdAsync(Guid seatId)
        {
            return await _context.Trips
                .Include(t => t.Seats)
                .FirstOrDefaultAsync(t => t.Seats.Any(s => s.Id == seatId));
        }
    }
}
