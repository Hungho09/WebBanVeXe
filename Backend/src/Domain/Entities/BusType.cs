using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class BusType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SeatCount { get; set; }
        public string? Description { get; set; }

        public ICollection<Bus> Buses { get; set; } = new List<Bus>();
    }
}
