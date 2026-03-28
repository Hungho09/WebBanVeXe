using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Location
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        // Flags/Badges
        public bool IsPickup { get; set; }
        public bool IsDropoff { get; set; }
        public string? Badge { get; set; } // "Gần bạn", "Phổ biến", etc.

        public Guid? ProvinceId { get; set; }
        public Province? Province { get; set; }
        
        public string? MapLink { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        
        public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
    }
}
