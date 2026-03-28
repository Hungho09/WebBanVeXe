using System;

namespace Domain.Entities
{
    public class RouteStop
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public Route Route { get; set; } = null!;
        public Guid LocationId { get; set; }
        public Location Location { get; set; } = null!;
        
        // Offset from trip departure time
        public int OffsetMinutes { get; set; }
        
        // Distance from origin in KM
        public double DistanceFromOriginKm { get; set; }
        
        // So we can order them
        public int OrderIndex { get; set; }
    }
}
