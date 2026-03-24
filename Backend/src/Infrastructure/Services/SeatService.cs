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

        public async Task<IEnumerable<SeatTemplate>> GetTemplatesByBusTypeAsync(Guid busTypeId)
        {
            return await _context.SeatTemplates
                .Where(t => t.BusTypeId == busTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Seat>> GenerateSeatsForTripAsync(Guid tripId, Guid busTypeId)
        {
            var templates = await GetTemplatesByBusTypeAsync(busTypeId);
            var busType = await _context.BusTypes.FindAsync(busTypeId);
            List<Seat> seats;

            if (templates != null && templates.Any())
            {
                // Generate actual seats based on the templates
                seats = templates.Select(t => new Seat
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
            }
            else if (busType != null)
            {
                // Fallback: Default layout calculation based on BusType Name
                seats = GenerateDefaultSeats(tripId, busType.Name);
            }
            else
            {
                seats = new List<Seat>();
            }

            // Save generated seats to database
            if (seats.Any())
            {
                // Prevent duplicate generation
                var existing = await _context.Seats.AnyAsync(s => s.TripId == tripId);
                if (!existing)
                {
                    await _context.Seats.AddRangeAsync(seats);
                    await _context.SaveChangesAsync();
                }
            }

            return seats;
        }

        private List<Seat> GenerateDefaultSeats(Guid tripId, string busTypeName)
        {
            var seats = new List<Seat>();
            var lowerName = busTypeName.ToLower();

            if (lowerName.Contains("sleeper") || lowerName.Contains("giường"))
            {
                // typically 2 floors, 3 columns (A, B, C) and 6 rows = 36 seats
                for (int floor = 1; floor <= 2; floor++)
                {
                    for (int col = 1; col <= 3; col++)
                    {
                        char colLetter = (char)('A' + col - 1);
                        for (int row = 1; row <= 6; row++)
                        {
                            seats.Add(new Seat {
                                Id = Guid.NewGuid(),
                                TripId = tripId,
                                SeatNumber = $"{floor}{colLetter}{row:00}",
                                Floor = floor,
                                RowNumber = row,
                                ColumnNumber = col,
                                Type = SeatType.Sleeper,
                                Status = SeatStatus.Available
                            });
                        }
                    }
                }
            }
            else if (lowerName.Contains("limousine"))
            {
                 // 9 Seat VIP (1 floor, 3 cols, 3 rows)
                for (int row = 1; row <= 3; row++)
                {
                    for (int col = 1; col <= 3; col++)
                    {
                        seats.Add(new Seat {
                            Id = Guid.NewGuid(),
                            TripId = tripId,
                            SeatNumber = $"L{row}{col}",
                            Floor = 1,
                            RowNumber = row,
                            ColumnNumber = col,
                            Type = SeatType.VIP,
                            Status = SeatStatus.Available
                        });
                    }
                }
            }
            else // Default to regular seats
            {
                // 45 Seat (1 floor, 5 cols, 9 rows)
                for (int row = 1; row <= 9; row++)
                {
                    for (int col = 1; col <= 5; col++)
                    {
                        seats.Add(new Seat {
                            Id = Guid.NewGuid(),
                            TripId = tripId,
                            SeatNumber = $"S{row:00}-{col}",
                            Floor = 1,
                            RowNumber = row,
                            ColumnNumber = col,
                            Type = SeatType.Normal,
                            Status = SeatStatus.Available
                        });
                    }
                }
            }
            
            return seats;
        }
    }
}
