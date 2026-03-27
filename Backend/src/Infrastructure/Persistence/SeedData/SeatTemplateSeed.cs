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

            // --- helpers ---
            void AddSeats(Guid typeId, int count, int columns, int floors, SeatType seatType, string prefix, string uuidPart) {
                int totalIdx = 1;
                int seatsPerFloor = (int)Math.Ceiling((double)count / floors);
                
                for (int f = 1; f <= floors; f++) {
                    int currentFloorCount = 0;
                    int r = 1;
                    while (currentFloorCount < seatsPerFloor && totalIdx <= count) {
                        for (int c = 1; c <= columns; c++) {
                            if (currentFloorCount >= seatsPerFloor || totalIdx > count) break;
                            
                            templates.Add(new SeatTemplate {
                                Id = Guid.Parse($"{uuidPart}-{totalIdx:D12}"),
                                BusTypeId = typeId,
                                SeatNumber = (floors > 1 ? (f == 1 ? "A" : "B") : (prefix)) + $"{currentFloorCount + 1:D2}",
                                RowNumber = r,
                                ColumnNumber = c,
                                Floor = f,
                                Type = (r == 1 && seatType == SeatType.Normal) ? SeatType.VIP : seatType
                            });
                            
                            currentFloorCount++;
                            totalIdx++;
                        }
                        r++;
                    }
                }
            }

            // 1. Xe ghế ngồi thông thường
            AddSeats(Guid.Parse("11000000-0000-0000-0000-000000000016"), 16, 4, 1, SeatType.Normal, "S", "11160000-0000-0000-0000");
            AddSeats(Guid.Parse("11000000-0000-0000-0000-000000000029"), 29, 4, 1, SeatType.Normal, "S", "11290000-0000-0000-0000");
            AddSeats(Guid.Parse("44444444-4444-4444-4444-444444444444"), 45, 5, 1, SeatType.Normal, "S", "45000000-0000-0000-0000");

            // 2. Xe Limousine ghế ngồi
            AddSeats(Guid.Parse("22222222-2222-2222-2222-222222222222"), 9, 3, 1, SeatType.VIP, "L", "90000000-0000-0000-0000");
            AddSeats(Guid.Parse("22000000-0000-0000-0000-000000000011"), 11, 3, 1, SeatType.VIP, "L", "22110000-0000-0000-0000");
            AddSeats(Guid.Parse("22000000-0000-0000-0000-000000000016"), 16, 4, 1, SeatType.VIP, "L", "22160000-0000-0000-0000");
            AddSeats(Guid.Parse("22000000-0000-0000-0000-000000000019"), 19, 4, 1, SeatType.VIP, "L", "22190000-0000-0000-0000");

            // 3. Xe giường nằm tiêu chuẩn
            AddSeats(Guid.Parse("33000000-0000-0000-0000-000000000034"), 34, 3, 2, SeatType.Sleeper, "A", "33340000-0000-0000-0000");
            AddSeats(Guid.Parse("33333333-3333-3333-3333-333333333333"), 44, 3, 2, SeatType.Sleeper, "A", "33440000-0000-0000-0000");

            // 4. Xe giường phòng / Cabin đơn
            AddSeats(Guid.Parse("55000000-0000-0000-0000-000000000020"), 20, 2, 2, SeatType.CabinSingle, "CS", "55200000-0000-0000-0000");
            AddSeats(Guid.Parse("55000000-0000-0000-0000-000000000024"), 24, 2, 2, SeatType.CabinSingle, "CS", "55240000-0000-0000-0000");

            // 5. Xe giường phòng / Cabin đôi
            AddSeats(Guid.Parse("66000000-0000-0000-0000-000000000022"), 22, 2, 2, SeatType.CabinDouble, "CD", "66220000-0000-0000-0000");
            AddSeats(Guid.Parse("66000000-0000-0000-0000-000000000024"), 24, 2, 2, SeatType.CabinDouble, "CD", "66240000-0000-0000-0000");

            modelBuilder.Entity<SeatTemplate>().HasData(templates);
        }
    }
}
