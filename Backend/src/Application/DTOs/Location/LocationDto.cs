using System;

namespace Application.DTOs.Location
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDropoff { get; set; }
        public string? Badge { get; set; }
        
        // New Module fields
        public Guid? ProvinceId { get; set; }
        public ProvinceDto? Province { get; set; }
        public string? MapLink { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}
