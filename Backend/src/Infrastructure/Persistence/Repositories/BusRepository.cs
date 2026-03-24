using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class BusRepository : IBusRepository
    {
        private readonly ApplicationDbContext _context;

        public BusRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bus>> GetAllAsync()
        {
            return await _context.Buses.Include(b => b.BusType).ToListAsync();
        }

        public async Task<Bus?> GetByIdAsync(Guid id)
        {
            return await _context.Buses.Include(b => b.BusType).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> ExistsByPlateNumberAsync(string plateNumber)
        {
            return await _context.Buses.AnyAsync(b => b.PlateNumber == plateNumber);
        }

        public async Task<Bus> AddAsync(Bus bus)
        {
            await _context.Buses.AddAsync(bus);
            await _context.SaveChangesAsync();
            return bus;
        }

        public async Task UpdateAsync(Bus bus)
        {
            _context.Buses.Update(bus);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Bus bus)
        {
            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();
        }
    }
}
