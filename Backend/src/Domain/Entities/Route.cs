using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Name => $"{Origin} - {Destination}";
        public string? Points { get; set; } = string.Empty;
        public int DistanceKm { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
