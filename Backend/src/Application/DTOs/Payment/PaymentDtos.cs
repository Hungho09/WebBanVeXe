using System;

namespace Application.DTOs.Payment
{
    public class PaymentRequestDto
    {
        public Guid BookingId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public class PaymentResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public Guid PaymentId { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
