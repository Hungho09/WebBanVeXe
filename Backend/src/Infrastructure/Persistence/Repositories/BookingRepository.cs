using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Seat)
                .Include(b => b.User)
                .Include(b => b.Trip)
                .Include(b => b.PickupPoint)
                .Include(b => b.DropoffPoint)
                .Include(b => b.Trip!)
                    .ThenInclude(t => t.Route)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Seat)
                .Include(b => b.Trip)
                .Include(b => b.PickupPoint)
                .Include(b => b.DropoffPoint)
                .Include(b => b.Trip!)
                    .ThenInclude(t => t.Route)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
