using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Invoice;
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

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _invoiceService.CreateForBookingAsync(request.BookingId, cancellationToken);
                return Ok(data);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create/{bookingId:guid}")]
        public async Task<IActionResult> CreateInvoiceByBookingId(Guid bookingId, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _invoiceService.CreateForBookingAsync(bookingId, cancellationToken);
                return Ok(data);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}/export/json")]
        public async Task<IActionResult> ExportJson(Guid id, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceService.GetByIdAsync(id, cancellationToken);
            if (invoice == null)
            {
                return NotFound();
            }

            var json = JsonSerializer.Serialize(invoice, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", $"invoice_{invoice.InvoiceNumber}.json");
        }

        [HttpGet("{id:guid}/export/pdf")]
        public async Task<IActionResult> ExportPdf(Guid id, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceService.GetByIdAsync(id, cancellationToken);
            if (invoice == null)
            {
                return NotFound();
            }

            // Create simple PDF content (you can use a proper PDF library like iTextSharp or PdfSharp)
            var pdfContent = GeneratePdfContent(invoice);
            var bytes = System.Text.Encoding.UTF8.GetBytes(pdfContent);
            
            return File(bytes, "application/pdf", $"invoice_{invoice.InvoiceNumber}.pdf");
        }

        private string GeneratePdfContent(InvoiceDto invoice)
        {
            // Simple HTML-to-PDF content (for demo)
            // In production, use proper PDF library
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Hóa đơn {invoice.InvoiceNumber}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ border-bottom: 2px solid #333; padding-bottom: 10px; margin-bottom: 20px; }}
        .invoice-info {{ margin-bottom: 20px; }}
        .customer-info {{ margin-bottom: 20px; }}
        .items {{ margin-bottom: 20px; }}
        .total {{ font-weight: bold; font-size: 18px; }}
        table {{ border-collapse: collapse; width: 100%; }}
        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
        th {{ background-color: #f2f2f22; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>HÓA ĐƠN</h1>
        <p><strong>Mã hóa đơn:</strong> {invoice.InvoiceNumber}</p>
        <p><strong>Ngày tạo:</strong> {invoice.CreatedAt:dd/MM/yyyy HH:mm}</p>
    </div>
    
    <div class='customer-info'>
        <h2>Thông tin khách hàng</h2>
        <p><strong>Họ tên:</strong> {invoice.CustomerName}</p>
        <p><strong>Email:</strong> {invoice.CustomerEmail}</p>
    </div>
    
    <div class='invoice-info'>
        <h2>Thông tin hóa đơn</h2>
        <p><strong>Mã đặt vé:</strong> {invoice.BookingId}</p>
        <p><strong>Tổng tiền:</strong> {invoice.TotalAmount:N0} VNĐ</p>
        <p><strong>Trạng thái:</strong> {invoice.Status}</p>
    </div>
    
    <div class='total'>
        <p><strong>TỔNG CỘNG: {invoice.TotalAmount:N0} VNĐ</strong></p>
    </div>
</body>
</html>";
            return html;
        }
    }

    public class CreateInvoiceRequest
    {
        public Guid BookingId { get; set; }
    }
}
