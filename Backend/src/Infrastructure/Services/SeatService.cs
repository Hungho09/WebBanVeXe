using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class SeatService : ISeatService
    {
        private readonly ApplicationDbContext _context;

        public SeatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SeatTemplate>> GetTemplatesByBusTypeAsync(BusType busType)
        {
            return await _context.SeatTemplates
                .Where(t => t.BusType == busType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Seat>> GenerateSeatsForTripAsync(Guid tripId, BusType busType)
        {
            var templates = await GetTemplatesByBusTypeAsync(busType);
            
            // Generate actual seats based on the templates
            var seats = templates.Select(t => new Seat
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                SeatNumber = t.SeatNumber,
                RowNumber = t.RowNumber,
                ColumnNumber = t.ColumnNumber,
                Floor = t.Floor,
                Type = t.Type,
                Status = SeatStatus.Available
            }).ToList();

            // Save generated seats to database
            await _context.Seats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();

            return seats;
        }
    }
}
