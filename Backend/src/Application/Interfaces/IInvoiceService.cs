using System;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInvoiceService
    {
        Task CreateInvoiceAsync(Guid bookingId);
        // (Optional) Task<byte[]> GeneratePdfInvoiceAsync(Guid invoiceId);
    }
}
