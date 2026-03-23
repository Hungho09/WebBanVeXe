using System;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Payment;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IApplicationDbContext _context;

        public PaymentService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto paymentRequestDto)
        {
            // Step 1: Check booking exists and status is Pending
            var booking = await _context.Bookings
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == paymentRequestDto.BookingId);

            if (booking == null)
            {
                throw new Exception("Booking not found.");
            }

            if (booking.BookingStatus != BookingStatus.Pending)
            {
                throw new Exception($"Booking status is {booking.BookingStatus}, but must be Pending to process payment.");
            }

            // Step 6: Use TRANSACTION
            using (var transaction = await _context.BeginTransactionAsync())
            {
                try
                {
                    // Step 2: Create new Payment (Status = Pending, TransactionCode = random)
                    var transactionCode = "PAY-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                    var payment = new Payment
                    {
                        Id = Guid.NewGuid(),
                        BookingId = booking.Id,
                        Amount = booking.TotalAmount,
                        PaymentMethod = paymentRequestDto.PaymentMethod,
                        PaymentStatus = PaymentStatus.Pending,
                        TransactionCode = transactionCode
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    // Step 3: Simulate success (Status = Success, PaidAt = current time)
                    payment.PaymentStatus = PaymentStatus.Success;
                    payment.PaidAt = DateTime.UtcNow;

                    // Step 4: Update BookingStatus = Confirmed
                    booking.BookingStatus = BookingStatus.Confirmed;

                    // Step 5: Update all related Seats Status = Booked
                    var bookingDetails = await _context.BookingDetails
                        .Where(bd => bd.BookingId == booking.Id)
                        .ToListAsync();

                    foreach (var detail in bookingDetails)
                    {
                        var seat = await _context.Seats.FindAsync(detail.SeatId);
                        if (seat != null)
                        {
                            seat.Status = SeatStatus.Booked;
                        }
                    }

                    await _context.SaveChangesAsync();
                    
                    // Commit transaction
                    await transaction.CommitAsync();

                    return new PaymentResponseDto
                    {
                        Message = "Payment processed successfully.",
                        PaymentId = payment.Id,
                        TransactionCode = payment.TransactionCode,
                        PaymentStatus = payment.PaymentStatus.ToString()
                    };
                }
                catch (Exception ex)
                {
                    // If any error occurs -> rollback automatically when using "using" or explicitly
                    await transaction.RollbackAsync();
                    throw new Exception($"Payment failed: {ex.Message}", ex);
                }
            }
        }
    }
}
