using System;
using Domain.Enums;

namespace Application.DTOs
{
    public class BusDto
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string BusType { get; set; } = string.Empty; // Return string for easier frontend read or enum int
        public int SeatCapacity { get; set; }
        public bool IsActive { get; set; }
    }
}
