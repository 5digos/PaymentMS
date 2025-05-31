using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces.IMapper;

namespace Application.Mappers
{
    public class PaymentMapper : IPaymentMapper
    {
        public Task<PaymentResponseDto> MapToResponseDto(Payment payment)
        {
            if (payment == null) return Task.FromResult<PaymentResponseDto>(null);

            var dto = new PaymentResponseDto
            {
                Id = payment.Id,
                Reference = payment.Reference,
                ExternalId = payment.ExternalId,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                CheckoutUrl = payment.CheckoutUrl,
                CreatedAt = payment.CreatedAt
            };
            return Task.FromResult(dto);
        }
    }
}
