using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Domain.Entities;

namespace Application.Interfaces.Gateway
{
    public interface IPaymentGateway
    {
        Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto request);
        Task<PaymentStatus> GetPaymentStatusAsync(string externalId);
        Task<Payment> ProcessWebhookAsync(string payload);
    }
}
