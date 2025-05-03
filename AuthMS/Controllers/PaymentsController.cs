using Microsoft.AspNetCore.Http;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IServices;
using Application.Dtos.Request;

namespace PaymentMS.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ICreatePaymentService _paymentService;
        private readonly IGetPaymentService _paymentGetService; 

        public PaymentsController(ICreatePaymentService paymentService, IGetPaymentService paymentGetService)
        {
            _paymentService = paymentService;
            _paymentGetService = paymentGetService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            try
            {
                var payment = await _paymentService.CreatePaymentAsync(request);
                return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var payment = await _paymentGetService.GetPaymentByIdAsync(id); 
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }
    }
}
