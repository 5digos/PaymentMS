using Microsoft.AspNetCore.Http;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IServices;
using Application.Dtos.Request;
using System.IO;
using System;
using System.Threading.Tasks;

namespace PaymentMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ICreatePaymentService _createPaymentService;
        private readonly IGetPaymentService _getPaymentService;
        private readonly IUpdatePaymentStatusService _updatePaymentService;

        public PaymentController(
            ICreatePaymentService createPaymentService,
            IGetPaymentService getPaymentService,
            IUpdatePaymentStatusService updatePaymentService)
        {
            _createPaymentService = createPaymentService;
            _getPaymentService = getPaymentService;
            _updatePaymentService = updatePaymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
        {
            try
            {
                var payment = await _createPaymentService.CreatePaymentAsync(request);
                return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var payment = await _getPaymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(Guid id, [FromBody] UpdatePaymentStatusRequestDto request)
        {
            try
            {
                if (id != request.PaymentId)
                {
                    return BadRequest("Payment ID mismatch.");
                }

                var success = await _updatePaymentService.UpdatePaymentStatusAsync(request);
                if (!success)
                {
                    return NotFound();
                }

                var updatedPayment = await _getPaymentService.GetPaymentByIdAsync(id);
                return Ok(updatedPayment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> ProcessWebhook()
        {
            try
            {
                string payload;
                using (var reader = new StreamReader(Request.Body))
                {
                    payload = await reader.ReadToEndAsync();
                }

                var payment = await _createPaymentService.ProcessWebhookAsync(payload);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
