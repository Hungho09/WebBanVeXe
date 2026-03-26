using System;
using System.Collections.Generic;
using Application.DTOs.Invoice;

namespace Application.DTOs.Booking
{
    public class CreateBookingDto
    {
        public Guid UserId { get; set; }
        public Guid TripId { get; set; }
        public List<Guid> SeatIds { get; set; } = new();
        public Guid? PickupPointId { get; set; }
        public Guid? DropoffPointId { get; set; }
    }

    public class BookingResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid TripId { get; set; }
        public decimal TotalAmount { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid? PickupPointId { get; set; }
        public string? PickupPointName { get; set; }
        public Guid? DropoffPointId { get; set; }
        public string? DropoffPointName { get; set; }
        public List<BookingDetailDto> Details { get; set; } = new();
        public InvoiceDto? Invoice { get; set; }
    }

    public class BookingDetailDto
    {
        public Guid Id { get; set; }
        public Guid SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
    }

    public class LockSeatRequestDto
    {
        public Guid UserId { get; set; }
    }
}
