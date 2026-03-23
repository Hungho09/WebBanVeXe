using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class SeatTemplate
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// Loại xe áp dụng layout này (Sleeper, Seat, Limousine)
        /// </summary>
        public BusType BusType { get; set; }
        
        /// <summary>
        /// Số ghế/Mã ghế tương đối (vd: A1, B2)
        /// </summary>
        public string SeatNumber { get; set; } = string.Empty;
        
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public int Floor { get; set; }
        
        /// <summary>
        /// Phân loại ghế (Thường, VIP) trong sơ đồ
        /// </summary>
        public SeatType Type { get; set; } = SeatType.Normal;
    }
}
