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
    public class PaymentStatusEntityMapper : IPaymentStatusEntityMapper
    {
        public Task<PaymentStatusResponseDto> MapToResponseDto(PaymentStatusEntity status)
        {
            if (status == null) return Task.FromResult<PaymentStatusResponseDto>(null);

            var dto = new PaymentStatusResponseDto
            {
                Id = status.Id,
                Name = status.Name,
                Description = status.Description
            };
            return Task.FromResult(dto);
        }
    }
}
