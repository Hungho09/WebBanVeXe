using System;

namespace Application.DTOs
{
    public class BusTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SeatCount { get; set; }
    }
}
