using System;

namespace Application.DTOs.Location
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ProvinceName { get; set; }
        public string? MapLink { get; set; }
    }
}
