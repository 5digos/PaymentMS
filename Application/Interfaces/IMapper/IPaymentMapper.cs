using System.Threading.Tasks;
using Application.Dtos.Response;
using Domain.Entities;

namespace Application.Interfaces.IMapper
{
    public interface IPaymentMapper
    {
        Task<PaymentResponseDto> MapToResponseDto(Payment payment);
    }
}
