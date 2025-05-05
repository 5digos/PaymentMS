using Microsoft.AspNetCore.Http;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IServices;
using Application.Dtos.Request;

namespace PaymentMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ICreatePaymentService _createPaymentService;
        private readonly IGetPaymentService _getPaymentService; 
        private readonly IUpdatePaymentStatusService _updatePaymentService;

        public PaymentController(ICreatePaymentService createPaymentService, IGetPaymentService getPaymentService, IUpdatePaymentStatusService updatePaymentService)
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
                var paymentId = await _createPaymentService.CreatePayment(request);
                return CreatedAtAction(nameof(GetPaymentById), new { id = paymentId }, paymentId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var payment = await _getPaymentService.GetPaymentById(id); 
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
                Console.WriteLine("Entró al PUT de PaymentController");

                if (id != request.PaymentId)
                {
                    return BadRequest("Payment ID mismatch.");
                }

                var success = await _updatePaymentService.UpdatePaymentStatus(request);
                if (success == false)
                {
                    Console.WriteLine("Error al actualizar el estado del pago.");
                    return NotFound();
                }
                // Traer el estado actualizado y devolverlo en el body
                var updatedPayment = await _getPaymentService.GetPaymentById(id);
                return Ok(updatedPayment);
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
