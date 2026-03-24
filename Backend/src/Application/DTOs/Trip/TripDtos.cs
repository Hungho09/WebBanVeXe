using System;
using Domain.Enums;

namespace Application.DTOs.Trip
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public string? RouteName { get; set; }
        public Guid BusId { get; set; }
        public string? BusPlate { get; set; }
        public string? BusTypeName { get; set; }
        public string? BusImageUrl { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTripDto
    {
        public Guid RouteId { get; set; }
        public Guid BusId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateTripDto
    {
        public Guid RouteId { get; set; }
        public Guid BusId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public TripStatus Status { get; set; }
    }
}
