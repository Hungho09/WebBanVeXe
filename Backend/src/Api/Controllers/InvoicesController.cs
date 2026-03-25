using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var data = await _invoiceService.GetAllAsync(cancellationToken);
            return Ok(data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var data = await _invoiceService.GetByIdAsync(id, cancellationToken);
            return data == null ? NotFound() : Ok(data);
        }

        [HttpGet("booking/{bookingId:guid}")]
        public async Task<IActionResult> GetByBookingId(Guid bookingId, CancellationToken cancellationToken)
        {
            var data = await _invoiceService.GetByBookingIdAsync(bookingId, cancellationToken);
            return data == null ? NotFound() : Ok(data);
        }
    }
}
