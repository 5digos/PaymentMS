using System.Threading.Tasks;
using Application.Dtos.Response;
using Domain.Entities;

namespace Application.Interfaces.IMapper
{
    public interface IPaymentMethodMapper
    {
        Task<PaymentMethodResponseDto> MapToResponseDto(PaymentMethod method);
    }
}
