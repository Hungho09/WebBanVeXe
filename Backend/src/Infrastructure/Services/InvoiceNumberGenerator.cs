using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class InvoiceNumberGenerator : IInvoiceNumberGenerator
    {
        private static readonly SemaphoreSlim Lock = new(1, 1);
        private readonly ApplicationDbContext _context;

        public InvoiceNumberGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
        {
            await Lock.WaitAsync(cancellationToken);
            try
            {
                var utcNow = DateTime.UtcNow;
                var datePart = utcNow.ToString("yyyyMMdd");
                var prefix = $"INV-{datePart}-";

                var lastToday = await _context.Invoices
                    .AsNoTracking()
                    .Where(x => x.InvoiceNumber.StartsWith(prefix))
                    .OrderByDescending(x => x.InvoiceNumber)
                    .Select(x => x.InvoiceNumber)
                    .FirstOrDefaultAsync(cancellationToken);

                var next = 1;
                if (!string.IsNullOrWhiteSpace(lastToday) && lastToday.Length >= prefix.Length + 4)
                {
                    var serialPart = lastToday.Substring(prefix.Length, 4);
                    if (int.TryParse(serialPart, out var parsed))
                    {
                        next = parsed + 1;
                    }
                }

                return $"{prefix}{next:0000}";
            }
            finally
            {
                Lock.Release();
            }
        }
    }
}
