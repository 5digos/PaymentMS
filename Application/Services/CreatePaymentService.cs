using System;
using System.Threading.Tasks;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces.Gateway;
using Application.Interfaces.ICommand;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.Services
{
    public class CreatePaymentService : ICreatePaymentService
    {
        private readonly IPaymentCommand _paymentCommand;
        private readonly IPaymentGateway _paymentGateway;

        public CreatePaymentService(IPaymentCommand paymentCommand, IPaymentGateway paymentGateway)
        {
            _paymentCommand = paymentCommand;
            _paymentGateway = paymentGateway;
        }

        public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto requestDto)
        {
            var currency = requestDto.Currency ?? "ARS";

            var gatewayRequest = new CreatePaymentRequestDto
            {
                Amount = requestDto.Amount,
                Currency = currency,
                Description = requestDto.Description,
                Reference = requestDto.Reference,
                CallbackUrl = requestDto.CallbackUrl,
                NotificationUrl = requestDto.NotificationUrl,
                Payer = new PayerInfoDto
                {
                    Email = requestDto.Payer?.Email,
                    Name = requestDto.Payer?.Name,
                    Identification = requestDto.Payer?.Identification,
                    IdentificationType = requestDto.Payer?.IdentificationType
                },
                Metadata = requestDto.Metadata
            };

            var gatewayResponse = await _paymentGateway.CreatePaymentAsync(gatewayRequest);

            var payment = new Payment(
                gatewayResponse.Id.ToString(),
                requestDto.Amount,
                currency,
                requestDto.Description
            );

            payment.SetExternalData(gatewayResponse.ExternalId, gatewayResponse.CheckoutUrl);

            await _paymentCommand.CreateAsync(payment);

            return new PaymentResponseDto
            {
                Id = payment.Id,
                Reference = payment.Reference,
                ExternalId = payment.ExternalId,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                CheckoutUrl = payment.CheckoutUrl,
                CreatedAt = payment.CreatedAt
            };
        }

        public async Task<PaymentStatus> GetPaymentStatusAsync(string externalId)
        {
            return await _paymentGateway.GetPaymentStatusAsync(externalId);
        }

        public async Task<Payment> ProcessWebhookAsync(string payload)
        {
            return await _paymentGateway.ProcessWebhookAsync(payload);
        }
    }
}