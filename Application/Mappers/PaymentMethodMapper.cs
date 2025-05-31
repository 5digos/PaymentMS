using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Dtos.Response;
using Application.Interfaces.IMapper;

namespace Application.Mappers
{
    public class PaymentMethodMapper : IPaymentMethodMapper
    {
        public Task<PaymentMethodResponseDto> MapToResponseDto(PaymentMethod method)
        {
            if (method == null) return Task.FromResult<PaymentMethodResponseDto>(null);

            var dto = new PaymentMethodResponseDto
            {
                Id = method.Id,
                Name = method.Name,
                Description = method.Description,
                IsActive = method.IsActive
            };
            return Task.FromResult(dto);
        }
    }
}
