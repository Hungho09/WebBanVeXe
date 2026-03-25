using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Invoice;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IApplicationDbContext _context;
        private readonly IInvoiceNumberGenerator _invoiceNumberGenerator;

        public InvoiceService(
            IApplicationDbContext context,
            IInvoiceNumberGenerator invoiceNumberGenerator)
        {
            _context = context;
            _invoiceNumberGenerator = invoiceNumberGenerator;
        }

        public async Task<InvoiceDto> CreateForBookingAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BookingId == bookingId, cancellationToken);
            if (existing != null)
            {
                return Map(existing);
            }

            var booking = await _context.Bookings
                .Include(b => b.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

            if (booking == null)
            {
                throw new InvalidOperationException("Booking not found when creating invoice.");
            }

            var number = await _invoiceNumberGenerator.GenerateAsync(cancellationToken);
            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = number,
                BookingId = booking.Id,
                CustomerName = booking.User?.FullName ?? booking.User?.UserName ?? "N/A",
                CustomerEmail = booking.User?.Email ?? string.Empty,
                TotalAmount = booking.TotalAmount,
                CreatedAt = DateTime.UtcNow,
                Status = Domain.Enums.InvoiceStatus.Paid
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(cancellationToken);

            return Map(invoice);
        }

        public async Task<InvoiceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return invoice == null ? null : Map(invoice);
        }

        public async Task<InvoiceDto?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BookingId == bookingId, cancellationToken);

            return invoice == null ? null : Map(invoice);
        }

        public async Task<IReadOnlyList<InvoiceDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var invoices = await _context.Invoices
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new InvoiceDto
                {
                    Id = x.Id,
                    InvoiceNumber = x.InvoiceNumber,
                    BookingId = x.BookingId,
                    CustomerName = x.CustomerName,
                    CustomerEmail = x.CustomerEmail,
                    TotalAmount = x.TotalAmount,
                    CreatedAt = x.CreatedAt,
                    Status = x.Status.ToString()
                })
                .ToListAsync(cancellationToken);

            return invoices;
        }

        private static InvoiceDto Map(Invoice x)
        {
            return new InvoiceDto
            {
                Id = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                BookingId = x.BookingId,
                CustomerName = x.CustomerName,
                CustomerEmail = x.CustomerEmail,
                TotalAmount = x.TotalAmount,
                CreatedAt = x.CreatedAt,
                Status = x.Status.ToString()
            };
        }
    }
}
