using Microsoft.AspNetCore.Http;
using Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IServices;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Domain.Entities;
using Application.Interfaces.IServices.IReservationServices;
using Application.UseCase;
using Infrastructure.HttpClients;

namespace PaymentMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ICreatePaymentService _createPaymentService;
        private readonly IGetPaymentService _getPaymentService; 
        private readonly IUpdatePaymentStatusService _updatePaymentService;
        private readonly MercadoPagoService _mercadoPagoService;
        private readonly IPaymentCalculationService _paymentCalculationService;
        private readonly IReservationServiceClient _reservationServiceClient;

        public PaymentController(
            ICreatePaymentService createPaymentService,
            IGetPaymentService getPaymentService,
            IUpdatePaymentStatusService updatePaymentService,
            MercadoPagoService mercadoPagoService,
            IPaymentCalculationService paymentCalculationService,
            IReservationServiceClient reservationServiceClient)
        {
            _createPaymentService = createPaymentService;
            _getPaymentService = getPaymentService;
            _updatePaymentService = updatePaymentService;
            _mercadoPagoService = mercadoPagoService; 
            _paymentCalculationService = paymentCalculationService;
            _reservationServiceClient = reservationServiceClient;
        }



        /// <summary>
        /// Creo un nuevo pago
        /// </summary>
        //[HttpPost]
        //public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
        //{
        //    try
        //    {
        //        var paymentId = await _createPaymentService.CreatePayment(request);
        //        return CreatedAtAction(nameof(GetPaymentById), new { id = paymentId }, paymentId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}


        /// <summary>
        /// Obtengo un pago por su ID
        /// </summary>
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



        /// <summary>
        /// Actualizo el estado de un pago
        /// </summary>
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




        /// <summary>
        /// Obtengo una reserva por su ID para efectuar el pago
        /// </summary>
        [HttpGet("reservation/{id}")]
        public async Task<IActionResult> GetReservationForPaymentById(Guid id)
        {
            var reservation = await _reservationServiceClient.GetReservationAsync(id); //obtengo reserva ya mapeada para usar en CreatePaymentFromReservation
            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }

        [HttpPost("from-reservation")]   //createPaymentDto es el dto que recibimos de reservas y se lo mandamos a MercadoPago
                                        //hay que crear un dto acorde para esto
        public async Task<IActionResult> CreatePaymentFromReservation([FromBody] ReservationSummaryResponse dto)
        {
            try
            {
                var amount = _paymentCalculationService.CalculateAmount(dto);
                var title = $"Reserva de vehículo {dto.VehicleId}"; // opcional, más personalizable

                // 1. Crear preferencia en Mercado Pago
                var checkoutUrl = await _mercadoPagoService.CreatePreferenceAsync(title, amount);

                // 2. Crear entidad Payment y guardarla (con estado pendiente)
                var payment = new Payment
                {
                    PaymentId = Guid.NewGuid(),
                    ReservationId = dto.ReservationId,
                    Amount = amount,
                    Date = DateTime.UtcNow,
                    PaymentMethodId = 1, // 1 = MercadoPago
                    PaymentStatusId = 1  // 1 = Pendiente, 2 = Aprobado, 3 = Rechazado
                };

                await _createPaymentService.SavePayment(payment); // guardo el nuevo pago

                return Ok(new
                {
                    checkoutUrl,
                    paymentId = payment.PaymentId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpPost("notifications")]
        public async Task<IActionResult> ReceiveNotification([FromQuery] string topic, [FromQuery] long id)
        {
            if (topic != "payment")
                return BadRequest("Not a payment notification.");

            try
            {
                // Obtener el detalle del pago desde MercadoPago
                var paymentInfo = await _mercadoPagoService.GetPaymentInfoAsync(id);

                // Buscar el Payment localmente por TransactionId
                var payment = await _paymentRepository.GetByTransactionId(paymentInfo.TransactionId);
                if (payment == null)
                    return NotFound("Payment not found.");

                // Actualizar estado según status de MercadoPago
                if (paymentInfo.Status == "approved")
                {
                    payment.PaymentStatusId = 2; // Aprobado

                    // Notificar a reserva
                    var confirmation = new PaymentConfirmationRequest
                    {
                        TotalAmount = payment.Amount,
                        LateFee = 0, // o lo que corresponda
                        PaymentGateway = "MercadoPago",
                        TransactionId = paymentInfo.TransactionId
                    };

                    await _reservationServiceClient.ConfirmPayment(payment.ReservationId, confirmation);
                }
                else if (paymentInfo.Status == "rejected" || paymentInfo.Status == "cancelled")
                {
                    payment.PaymentStatusId = 3; // Rechazado
                }

                await _paymentRepository.UpdateAsync(payment);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
