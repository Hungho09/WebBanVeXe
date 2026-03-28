using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TripId { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Guid? PickupPointId { get; set; }
        public Guid? DropoffPointId { get; set; }
        
        public Location? PickupPoint { get; set; }
        public Location? DropoffPoint { get; set; }

        public User? User { get; set; }
        public Trip? Trip { get; set; }
        public Invoice? Invoice { get; set; }
        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    }
}
