using System;
using Domain.Enums;

namespace Application.DTOs
{
    public class BusDto
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int SeatCount { get; set; }
        public BusTypeDto BusType { get; set; } = null!;
        public bool IsActive { get; set; }
        public BusStatus Status { get; set; } = BusStatus.Available;
        public string StatusLabel => Status switch
        {
            BusStatus.Active => "Đang hoạt động",
            BusStatus.Available => "Có sẵn",
            BusStatus.Inactive => "Ngưng hoạt động",
            _ => "Không xác định"
        };
    }
}
