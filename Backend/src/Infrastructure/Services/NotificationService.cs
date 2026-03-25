using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendBookingConfirmationAsync(Guid bookingId)
        {
            // Simulated Email Sending
            _logger.LogInformation("Sending Booking Confirmation Email for Booking ID: {BookingId}...", bookingId);
            await Task.Delay(500); // Simulate network delay
            _logger.LogInformation("Email sent successfully to customer.");
        }

        public async Task SendCancellationApprovalAsync(Guid bookingId)
        {
            // Simulated Email Sending for Cancellation
            _logger.LogInformation("Sending Cancellation Approval Email for Booking ID: {BookingId}...", bookingId);
            await Task.Delay(500);
            _logger.LogInformation("Cancellation Email sent successfully.");
        }
    }
}
