using System;

namespace Application.DTOs.Trip
{
    public class SeatDto
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public int Floor { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime? LockExpirationTime { get; set; }
        public Guid? LockedByUserId { get; set; }
    }
}
