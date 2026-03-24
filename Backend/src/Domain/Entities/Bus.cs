using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class Bus
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        
        public Guid BusTypeId { get; set; }
        public BusType BusType { get; set; } = null!;
        
        public int SeatCount { get; set; }
        
        // 3-state status: Active (on trip), Available (free for new trip), Inactive (decommissioned)
        public BusStatus Status { get; set; } = BusStatus.Available;
        public bool IsActive => Status != BusStatus.Inactive;

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
