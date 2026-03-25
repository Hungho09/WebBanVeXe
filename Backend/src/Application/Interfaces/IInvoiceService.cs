using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Invoice;

namespace Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> CreateForBookingAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task<InvoiceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<InvoiceDto?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<InvoiceDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
