using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public string TransactionCode { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }

        public Booking? Booking { get; set; }
    }
}
