using System;

namespace Application.DTOs.Payment
{
    public class CreatePaymentDto
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionCode { get; set; } = string.Empty;
    }

    public class PaymentResultDto
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionCode { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
    }
}
