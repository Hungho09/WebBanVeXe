using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Invoice;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
            QuestPDF.Settings.License = LicenseType.Community;
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

            var pdfBytes = GeneratePdfContent(invoice);
            
            return File(pdfBytes, "application/pdf", $"invoice_{invoice.InvoiceNumber}.pdf");
        }

        private byte[] GeneratePdfContent(InvoiceDto invoice)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));

                    page.Header().Element(header => 
                    {
                        header.Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("HÓA ĐƠN GIAO DỊCH").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                                col.Item().Text($"Mã hóa đơn: {invoice.InvoiceNumber}").FontSize(14);
                                col.Item().Text($"Ngày tạo: {invoice.CreatedAt:dd/MM/yyyy HH:mm}");
                            });
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text("Thông tin khách hàng").SemiBold().FontSize(14);
                        col.Item().Text($"Họ tên: {invoice.CustomerName}");
                        col.Item().Text($"Email: {invoice.CustomerEmail}");
                        
                        col.Item().PaddingTop(20).PaddingBottom(10).Text("Thông tin đặt vé").SemiBold().FontSize(14);
                        col.Item().Text($"Mã đặt vé: {invoice.BookingId}");
                        col.Item().Text($"Trạng thái: {invoice.Status}");
                        
                        col.Item().PaddingTop(30).Row(row => 
                        {
                            row.RelativeItem().Text("TỔNG CỘNG:").SemiBold().FontSize(16).AlignRight();
                            row.ConstantItem(150).Text($"{invoice.TotalAmount:N0} VNĐ").SemiBold().FontSize(16).AlignRight().FontColor(Colors.Red.Medium);
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Cảm ơn quý khách đã sử dụng dịch vụ - Trang ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }

    public class CreateInvoiceRequest
    {
        public Guid BookingId { get; set; }
    }
}
