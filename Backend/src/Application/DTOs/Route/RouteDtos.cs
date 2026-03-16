using System;

namespace Application.DTOs.Route
{
    public class RouteDto
    {
        public Guid Id { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Points { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int DistanceKm { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateRouteDto
    {
        public string Origin { get; set; } = string.Empty;
        public string Points { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int DistanceKm { get; set; }
    }

    public class UpdateRouteDto
    {
        public string Origin { get; set; } = string.Empty;
        public string Points { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int DistanceKm { get; set; }
        public bool IsActive { get; set; }
    }
}
