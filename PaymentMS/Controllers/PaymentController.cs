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

        public PaymentController(
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
        }


        /// <summary>
        /// Obtengo una reserva por su ID a la cual efectuarle el pago
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


        /// <summary>
        /// Crea un pago a partir de una reserva
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("from-reservation")]
        public async Task<IActionResult> CreatePaymentFromReservation([FromBody] ReservationSummaryResponse dto)
        {
            try
            {
                var (totalAmount, lateFee) = _paymentCalculationService.CalculateAmount(dto);

                var title = $"Pago de la Reserva del vehículo:  ";

                var paymentId = Guid.NewGuid();

                var checkoutUrl = await _mercadoPagoService.CreatePreferenceAsync(title, totalAmount, paymentId, lateFee);

                var payment = new Payment
                {
                    PaymentId = paymentId,
                    ReservationId = dto.ReservationId,
                    Amount = totalAmount,
                    Date = DateTime.UtcNow,
                    PaymentMethodId = 1,
                    PaymentStatusId = 1
                };

                await _createPaymentService.SavePayment(payment);

                // Agregamos las BackUrls como parte de la respuesta
                string baseUrl = "http://127.0.0.1:5501/front";

                var successUrl = baseUrl + "/verify.html";
                var failureUrl = baseUrl + "/failure.html";
                var pendingUrl = baseUrl + "/pending.html";

                return Ok(new
                {
                    paymentLink = checkoutUrl,
                    paymentId = payment.PaymentId,
                    backUrls = new
                    {
                        success = successUrl,
                        failure = failureUrl,
                        pending = pendingUrl
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.ToString() });
            }
        }




        /// <summary>
        /// Verificar el pago aprobado en MercadoPago y actualizar el estado en la base de datos, notificando al microservicio de reservas, y cerrando el flujo.
        /// </summary>
        /// <param name="mercadoPagoPaymentId"></param>
        /// <returns></returns>
        [HttpPost("verify/{mercadoPagoPaymentId:long}")]
        public async Task<IActionResult> VerifyPayment(long mercadoPagoPaymentId)
        {
            try
            {
                // Obtener el detalle del pago desde MercadoPago
                var paymentInfoMP = await _mercadoPagoService.GetPaymentInfoAsync(mercadoPagoPaymentId);
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
                        $"-Id del pago en bd:  {paymentId}\n" +
                        $"-Id de la reserva asociada a este pago: {payment.ReservationId}\n" +
                        $"-Estado del pago actualizado a '{paymentInfoMP.Status}'.\n" +
                        $"-Pago total: {payment.Amount}.\n" +
                        $"-Tarifa adicional cobrada por demora: {lateFee}.\n" +
                        $"-Transaccion Id de MercadoPago: {paymentInfoMP.TransactionId}."
                        );
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
        [HttpGet("pago-exitoso")]
        public async Task<IActionResult> PagoExitoso(
                [FromQuery(Name = "payment_id")] long paymentId)
        //[FromQuery(Name = "external_reference")] string externalReference*/)
        {
            //return Redirect($"/api/payment/verify-from-get?payment_id={paymentId}"); // Redirigís directamente al nuevo GET que ejecuta VerifyPayment
            return await VerifyPayment(paymentId);
        }

        /// <summary>
        /// Obtengo un pago por su ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var payment = await _getPaymentService.GetPaymentResponseDtoById(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        //// GET: /api/payment/verify-from-get
        //[HttpGet("verify-from-get")]
        //public async Task<IActionResult> VerifyFromGet([FromQuery(Name = "payment_id")] long mercadoPagoPaymentId)
        //{
        //    // Llamás al mismo servicio que el endpoint POST hace
        //    return await VerifyPayment(paymentId);;
        //}

    }
}