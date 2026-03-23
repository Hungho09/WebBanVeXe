using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Payment;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentResultDto>> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
        {
            try
            {
                var result = await _paymentService.CreatePaymentAsync(createPaymentDto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("{id}/process")]
        public async Task<ActionResult<PaymentResultDto>> ProcessPayment(Guid id)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentResultDto>> GetById(Guid id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<ActionResult<IEnumerable<PaymentResultDto>>> GetByBookingId(Guid bookingId)
        {
            var result = await _paymentService.GetPaymentsByBookingIdAsync(bookingId);
            return Ok(result);
        }

        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelPayment(Guid id)
        {
            var success = await _paymentService.CancelPaymentAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
