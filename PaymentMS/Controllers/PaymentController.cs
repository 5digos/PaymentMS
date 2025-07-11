﻿
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IServices;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Domain.Entities;
using Application.Interfaces.IServices.IReservationServices;
using Application.Interfaces.ICommand;
using Infrastructure.HttpClients.Dtos;
using System.Text.Json;

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
        private readonly IPaymentCommand _paymentCommand;
        private readonly ILogger<PaymentController> _logger;
        private readonly string _baseUrl;

        public PaymentController(
            IConfiguration configuration,
            ICreatePaymentService createPaymentService,
            IGetPaymentService getPaymentService,
            IUpdatePaymentStatusService updatePaymentService,
            MercadoPagoService mercadoPagoService,
            IPaymentCalculationService paymentCalculationService,
            IReservationServiceClient reservationServiceClient,
            IPaymentCommand paymentCommand,
            ILogger<PaymentController> logger)
        {
            _createPaymentService = createPaymentService;
            _getPaymentService = getPaymentService;
            _updatePaymentService = updatePaymentService;
            _mercadoPagoService = mercadoPagoService;
            _paymentCalculationService = paymentCalculationService;
            _reservationServiceClient = reservationServiceClient;
            _paymentCommand = paymentCommand;
            _logger = logger;
            _baseUrl = configuration["MercadoPago:BaseUrl"];
        }


        /// <summary>
        /// Obtengo una reserva por su ID a la cual efectuarle el pago
        /// </summary>
        //[Authorize(Policy = "ActiveUser")]
        [AllowAnonymous]
        [HttpGet("reservation/{id}")]
        public async Task<IActionResult> GetReservationForPaymentById(Guid id)
        {
            ////// Extraer UserId del JWT
            //var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "UserId");
            //if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            //    return Forbid();

            var reservation = await _reservationServiceClient.GetReservationAsync(id); //obtengo reserva ya mapeada para usar en CreatePaymentFromReservation
            if (reservation == null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }


        /// <summary>
        /// Crea un pago a partir de una reserva
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        //[Authorize(Policy = "ActiveUser")]
        [AllowAnonymous]
        [HttpPost("from-reservation")]//ReservationSummaryResponse es el que recibimos de reservas y se lo mandamos a MercadoPago, para crear el pago y el checkoutPro
        public async Task<IActionResult> CreatePaymentFromReservation([FromBody] ReservationSummaryResponse dto)
        {
            try
            {
                ////// Extraer UserId del JWT
                //var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "UserId");
                //if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                //    return Forbid();

                var (totalAmount, lateFee) = _paymentCalculationService.CalculateAmount(dto); // Calculo el total a pagar y la tarifa de retraso

                var title = $"Pago de la Reserva del vehículo:  ";

                var paymentId = Guid.NewGuid(); //creo un id de pago para la db local

                // 1. Crear preferencia en Mercado Pago
                var checkoutUrl = await _mercadoPagoService.CreatePreferenceAsync(title, totalAmount, paymentId, lateFee);

                // 2. Crear entidad Payment y guardarla (con estado pendiente)
                var payment = new Payment
                {
                    PaymentId = paymentId,
                    ReservationId = dto.ReservationId,
                    Amount = totalAmount,
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
                _logger.LogError(ex, "Error en CreatePaymentFromReservation");
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Verificar el pago aprobado en MercadoPago y actualizar el estado en la base de datos, notificando al microservicio de reservas, y cerrando el flujo.
        /// </summary>
        /// <param name="mercadoPagoPaymentId"></param>
        /// <returns></returns>
        //[Authorize(Policy = "ActiveUser")]
        [AllowAnonymous]
        [HttpPost("verify/{mercadoPagoPaymentId:long}")]
        public async Task<IActionResult> VerifyPayment(long mercadoPagoPaymentId)
        {
            try
            {
                ////// Extraer UserId del JWT
                //var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "UserId");
                //if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                //    return Forbid();

                // Obtener el detalle del pago desde MercadoPago
                var paymentInfoMP = await _mercadoPagoService.GetPaymentInfoAsync(mercadoPagoPaymentId);
                _logger.LogInformation($"PaymentInfo: {paymentInfoMP}");
                if (paymentInfoMP == null)
                    return NotFound("No se encontró información del pago en MercadoPago");

                // Deserializar el ExternalReference para obtener el PaymentId y la LateFee
                var referenceData = JsonSerializer.Deserialize<PaymentReferenceData>(paymentInfoMP.ExternalReference);
                var paymentId = referenceData.PaymentId;
                var lateFee = referenceData.LateFee;

                var payment = await _getPaymentService.GetPaymentByIdAsync(paymentId);
                if (payment == null)
                    return NotFound("Pago no encontrado en la base de datos local.");

                if (payment.PaymentStatusId == 2 || payment.PaymentStatusId == 3)
                    return Ok("El pago ya fue procesado.");

                // Actualizar estado según status de MercadoPago
                if (paymentInfoMP.Status == "approved")
                {
                    payment.PaymentStatusId = 2; // Aprobado

                    // Notificar a reserva
                    var confirmation = new PaymentConfirmationRequest
                    {
                        TotalAmount = payment.Amount,
                        LateFee = lateFee,
                        PaymentGateway = "MercadoPago",
                        TransactionId = paymentInfoMP.TransactionId
                    };

                    try
                    {
                        await _reservationServiceClient.ConfirmPayment(payment.ReservationId, confirmation);
                    }
                    catch (Exception notifyEx)
                    {
                        _logger.LogError(notifyEx, "Error notificando al microservicio de reservas");
                    }
                }
                else if (paymentInfoMP.Status == "rejected" || paymentInfoMP.Status == "cancelled")
                {
                    payment.PaymentStatusId = 3; // Rechazado
                }

                await _paymentCommand.UpdatePaymentAsync(payment);
                return Ok(
                    new
                    {
                        PaymentId = paymentId,
                        ReservationId = payment.ReservationId,
                        //Status = paymentInfoMP.Status,
                        Amount = payment.Amount,
                        LateFee = lateFee,
                        TransactionId = paymentInfoMP.TransactionId
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar notificación de pago");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// MP ejecuta este GET automaticamente cuando el pago es aprobado y ejecuta VerifyPayment, confirmando el pago y notificando al microservicio de reservas
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        /// GET: /api/payment/pago-exitoso
        //[Authorize(Policy = "ActiveUser")]
        [AllowAnonymous]
        [HttpGet("pago-exitoso")]
        public async Task<IActionResult> PagoExitoso(
                [FromQuery(Name = "payment_id")] long paymentId)
        {
            //var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "UserId");
            //if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            //    return Forbid();

            //return await VerifyPayment(paymentId);
            return Redirect($"{_baseUrl}/payment-success.html?payment_id={paymentId}");
        }

        /// <summary>
        /// Obtengo un pago por el ID de la reserva
        /// </summary>
        //[Authorize(Policy = "ActiveUser")]
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPaymentByReservationId(Guid id)
        {
            ////// Extraer UserId del JWT
            //var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "UserId");
            //if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            //    return Forbid();

            var payment = await _getPaymentService.GetPaymentDtoByReservationId(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }
    }
}
