using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class Bus
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public BusType BusType { get; set; }
        public int SeatCapacity { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
