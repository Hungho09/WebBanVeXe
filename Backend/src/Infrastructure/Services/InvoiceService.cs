using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateInvoiceAsync(Guid bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return;

            // Generate unique invoice number (Epic 3.5)
            var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                InvoiceNumber = invoiceNumber,
                CustomerName = booking.User?.UserName ?? "N/A",
                CustomerEmail = booking.User?.Email ?? "N/A",
                TotalAmount = booking.TotalAmount,
                CreatedAt = DateTime.UtcNow,
                Status = InvoiceStatus.Paid
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
        }
    }
}
