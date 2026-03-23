using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Payment;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentResultDto> CreatePaymentAsync(CreatePaymentDto createPaymentDto)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == createPaymentDto.BookingId);

            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {createPaymentDto.BookingId} not found.");
            }

            if (booking.BookingStatus == BookingStatus.Confirmed || booking.BookingStatus == BookingStatus.Cancelled)
            {
                throw new InvalidOperationException($"Booking is already {booking.BookingStatus}.");
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = createPaymentDto.BookingId,
                Amount = createPaymentDto.Amount,
                PaymentMethod = createPaymentDto.PaymentMethod,
                TransactionCode = createPaymentDto.TransactionCode,
                PaymentStatus = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return MapToDto(payment, booking);
        }

        public async Task<PaymentResultDto> ProcessPaymentAsync(Guid id)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {id} not found.");
            }

            if (payment.PaymentStatus != PaymentStatus.Pending)
            {
                throw new InvalidOperationException($"Payment is already {payment.PaymentStatus}.");
            }

            payment.PaymentStatus = PaymentStatus.Success;
            payment.PaidAt = DateTime.UtcNow;

            if (payment.Booking != null)
            {
                payment.Booking.BookingStatus = BookingStatus.Confirmed;
            }

            await _context.SaveChangesAsync();

            return MapToDto(payment, payment.Booking!);
        }

        public async Task<PaymentResultDto> UpdatePaymentStatusAsync(Guid id, PaymentStatus status)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {id} not found.");
            }

            payment.PaymentStatus = status;

            if (payment.Booking != null)
            {
                if (status == PaymentStatus.Success)
                {
                    payment.Booking.BookingStatus = BookingStatus.Confirmed;
                }
                else if (status == PaymentStatus.Failed)
                {
                    payment.Booking.BookingStatus = BookingStatus.Cancelled;
                }
            }

            await _context.SaveChangesAsync();

            return MapToDto(payment, payment.Booking!);
        }

        public async Task<PaymentResultDto?> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null) return null;

            return MapToDto(payment, payment.Booking!);
        }

        public async Task<IEnumerable<PaymentResultDto>> GetPaymentsByBookingIdAsync(Guid bookingId)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                .Where(p => p.BookingId == bookingId)
                .Select(p => MapToDto(p, p.Booking!))
                .ToListAsync();
        }

        public async Task<bool> CancelPaymentAsync(Guid id)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null) return false;

            payment.PaymentStatus = PaymentStatus.Failed;
            
            if (payment.Booking != null)
            {
                payment.Booking.BookingStatus = BookingStatus.Pending;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private PaymentResultDto MapToDto(Payment payment, Booking booking)
        {
            return new PaymentResultDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                TransactionCode = payment.TransactionCode,
                PaymentStatus = payment.PaymentStatus.ToString(),
                BookingStatus = booking.BookingStatus.ToString(),
                PaidAt = payment.PaidAt
            };
        }
    }
}
