using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Payment;

using Domain.Enums;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResultDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto);
        Task<PaymentResultDto> ProcessPaymentAsync(Guid id);
        Task<PaymentResultDto> UpdatePaymentStatusAsync(Guid id, PaymentStatus status);
        Task<PaymentResultDto?> GetPaymentByIdAsync(Guid id);
        Task<IEnumerable<PaymentResultDto>> GetPaymentsByBookingIdAsync(Guid bookingId);
        Task<bool> CancelPaymentAsync(Guid id);
    }
}
