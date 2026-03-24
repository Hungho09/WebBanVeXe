using Application.DTOs;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BusTypeService : IBusTypeService
    {
        private readonly IApplicationDbContext _context;

        public BusTypeService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BusTypeDto>> GetAllAsync()
        {
            var busTypes = await _context.BusTypes.ToListAsync();
            return busTypes.Select(bt => new BusTypeDto
            {
                Id = bt.Id,
                Name = bt.Name,
                SeatCount = bt.SeatCount
            });
        }
    }
}
