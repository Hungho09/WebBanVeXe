using System;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task SendBookingConfirmationAsync(Guid bookingId);
        Task SendCancellationApprovalAsync(Guid bookingId);
    }
}
