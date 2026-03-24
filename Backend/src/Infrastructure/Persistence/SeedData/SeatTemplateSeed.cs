using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.SeedData
{
    public static class SeatTemplateSeed
    {
        private static readonly Guid LimousineTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid SleeperTypeId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        private static readonly Guid SeatTypeId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        public static void Seed(ModelBuilder modelBuilder)
        {
            var templates = new List<SeatTemplate>();

            // --- 1. Limousine (9 seats, VIP) ---
            int lCount = 1;
            for (int r = 1; r <= 3; r++)
            {
                for (int c = 1; c <= 3; c++)
                {
                    templates.Add(new SeatTemplate
                    {
                        Id = Guid.Parse($"90000000-0000-0000-0000-{lCount:D12}"),
                        BusTypeId = LimousineTypeId,
                        SeatNumber = $"L{lCount:D2}",
                        RowNumber = r,
                        ColumnNumber = c,
                        Floor = 1,
                        Type = SeatType.VIP
                    });
                    lCount++;
                }
            }

            // --- 2. Sleeper (36 seats, 2 floors) ---
            int sIdx = 1;
            for (int f = 1; f <= 2; f++)
            {
                char prefix = f == 1 ? 'A' : 'B';
                int currentFloorSeatIdx = 1;
                for (int r = 1; r <= 6; r++)
                {
                    for (int c = 1; c <= 3; c++)
                    {
                        templates.Add(new SeatTemplate
                        {
                            Id = Guid.Parse($"{f}0000000-0000-0000-0000-{sIdx:D12}"),
                            BusTypeId = SleeperTypeId,
                            SeatNumber = $"{prefix}{currentFloorSeatIdx:D2}",
                            RowNumber = r,
                            ColumnNumber = c,
                            Floor = f,
                            Type = r == 1 ? SeatType.VIP : SeatType.Normal // First row is VIP
                        });
                        currentFloorSeatIdx++;
                        sIdx++;
                    }
                }
            }

            // --- 3. Seat (45 seats, 1 floor) ---
            int seatIdx45 = 1;
            for (int r = 1; r <= 9; r++)
            {
                for (int c = 1; c <= 5; c++)
                {
                    templates.Add(new SeatTemplate
                    {
                        Id = Guid.Parse($"45000000-0000-0000-0000-{seatIdx45:D12}"),
                        BusTypeId = SeatTypeId,
                        SeatNumber = $"S{seatIdx45:D2}",
                        RowNumber = r,
                        ColumnNumber = c,
                        Floor = 1,
                        Type = r <= 2 ? SeatType.VIP : SeatType.Normal // First 2 rows are VIP
                    });
                    seatIdx45++;
                }
            }

            modelBuilder.Entity<SeatTemplate>().HasData(templates);
        }
    }
}
