using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class Seat
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public int Floor { get; set; }
        public SeatType Type { get; set; } = SeatType.Normal;
        public SeatStatus Status { get; set; } = SeatStatus.Available;
        public DateTime? LockExpirationTime { get; set; }
        /// <summary>User đang giữ ghế tạm (khi Status = Locked và chưa hết hạn).</summary>
        public Guid? LockedByUserId { get; set; }

        public Trip? Trip { get; set; }
        
        [System.ComponentModel.DataAnnotations.Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
