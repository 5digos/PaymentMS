using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Configuration;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces.Gateway;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.Services
{
    public class MercadoPagoService : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;
        private readonly string _baseUrl = "https://api.mercadopago.com/checkout/preferences";

        public MercadoPagoService(HttpClient httpClient, AppSettings appSettings)
        {
            _httpClient = httpClient;
            _accessToken = appSettings["MercadoPago:AccessToken"];

            // Configurar headers por defecto
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
        }

        public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            try
            {
                var mpRequest = new
                {
                    items = new[]
                    {
                new {
                    id = request.Reference,
                    title = request.Description,
                    description = request.Description,
                    quantity = 1,
                    unit_price = request.Amount,
                    currency_id = request.Currency ?? "ARS" // Valor predeterminado si no se proporciona
                }
            },
                    payer = new
                    {
                        email = request.Payer?.Email,
                        name = request.Payer?.Name,
                        identification = new
                        {
                            type = request.Payer?.IdentificationType,
                            number = request.Payer?.Identification
                        }
                    },
                    back_urls = new
                    {
                        success = request.CallbackUrl + "?status=success",
                        failure = request.CallbackUrl + "?status=failure",
                        pending = request.CallbackUrl + "?status=pending"
                    },
                    notification_url = request.NotificationUrl,
                    external_reference = request.Reference,
                    auto_return = "approved",
                    metadata = request.Metadata ?? new Dictionary<string, object>()
                };

                var response = await _httpClient.PostAsJsonAsync(_baseUrl, mpRequest);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadFromJsonAsync<JsonElement>();

                // Verificamos que las propiedades existan antes de acceder a ellas
                if (!content.TryGetProperty("id", out var idProperty))
                {
                    throw new InvalidOperationException("La respuesta de Mercado Pago no contiene un ID");
                }

                if (!content.TryGetProperty("init_point", out var initPointProperty))
                {
                    throw new InvalidOperationException("La respuesta de Mercado Pago no contiene una URL de checkout");
                }

                string externalId = idProperty.ValueKind == JsonValueKind.String
                    ? idProperty.GetString()
                    : idProperty.GetInt64().ToString();

                string checkoutUrl = initPointProperty.GetString();

                return new PaymentResponseDto
                {
                    Id = Guid.NewGuid(), // Generamos un ID interno para nuestro sistema
                    Reference = request.Reference,
                    ExternalId = externalId,
                    Status = "pending",
                    CheckoutUrl = checkoutUrl,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (HttpRequestException ex)
            {
                // Log the exception
                Console.WriteLine($"Error al comunicarse con Mercado Pago: {ex.Message}");
                throw new Exception("Error al comunicarse con Mercado Pago", ex);
            }
            catch (JsonException ex)
            {
                // Log the exception
                Console.WriteLine($"Error al procesar la respuesta de Mercado Pago: {ex.Message}");
                throw new Exception("Error al procesar la respuesta de Mercado Pago", ex);
            }
        }


        public async Task<PaymentStatus> GetPaymentStatusAsync(string externalId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{externalId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            var status = content.GetProperty("status").GetString();

            return MapStatus(status);
        }

        public async Task<Payment> ProcessWebhookAsync(string payload)
        {
            var data = JsonSerializer.Deserialize<JsonElement>(payload);

            // Verificar tipo de notificación
            if (data.TryGetProperty("type", out var type) && type.GetString() == "payment")
            {
                if (data.TryGetProperty("data", out var paymentData))
                {
                    var paymentId = paymentData.GetProperty("id").GetString();

                    // Consultar estado del pago a Mercado Pago
                    var paymentResponse = await _httpClient.GetAsync($"https://api.mercadopago.com/v1/payments/{paymentId}");
                    paymentResponse.EnsureSuccessStatusCode();

                    var paymentInfo = await paymentResponse.Content.ReadFromJsonAsync<JsonElement>();
                    var status = paymentInfo.GetProperty("status").GetString();
                    var externalReference = paymentInfo.GetProperty("external_reference").GetString();

                    // Se crea una entidad de pago incompleta que será actualizada por el caso de uso
                    var payment = new Payment(
                        paymentId, // Usa el ID del pago de Mercado Pago
                        paymentInfo.GetProperty("transaction_amount").GetDecimal(),
                        paymentInfo.GetProperty("currency_id").GetString(),
                        externalReference
                    );

                    // Actualizamos el estado según la respuesta
                    switch (status)
                    {
                        case "approved":
                            payment.Complete();
                            break;
                        case "rejected":
                            payment.Fail();
                            break;
                        case "cancelled":
                            payment.Cancel();
                            break;
                    }

                    return payment;
                }
            }

            throw new InvalidOperationException("Notificación no procesable");
        }

        private PaymentStatus MapStatus(string mpStatus)
        {
            return mpStatus switch
            {
                "approved" => PaymentStatus.Completed,
                "rejected" => PaymentStatus.Failed,
                "cancelled" => PaymentStatus.Cancelled,
                _ => PaymentStatus.Pending
            };
        }
    }
}