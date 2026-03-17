using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class Trip
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public Guid BusId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public TripStatus Status { get; set; } = TripStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Route? Route { get; set; }
        public Bus? Bus { get; set; }
    }
}
