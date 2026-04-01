using System;

namespace Domain.Entities
{
    public class RouteStop
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public Route Route { get; set; } = null!;
        public Guid LocationId { get; set; }        
        public Location Location { get; set; } = null!;  // ← giữ tên cũ
        public int OffsetMinutes { get; set; }
        public double DistanceFromOriginKm { get; set; }
        public int OrderIndex { get; set; }
    }
}