using System;
using System.Threading.Tasks;
using Application.DTOs.Payment;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto paymentRequestDto);
    }
}
