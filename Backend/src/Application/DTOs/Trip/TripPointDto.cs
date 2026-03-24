using System;

namespace Application.DTOs.Trip
{
    public class TripPointDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime ExpectedTime { get; set; }
        public double DistanceFromOrigin { get; set; }
        public string? Badge { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDropoff { get; set; }
    }
}
