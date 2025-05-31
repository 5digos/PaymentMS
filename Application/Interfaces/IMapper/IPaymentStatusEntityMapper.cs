using System.Threading.Tasks;
using Application.Dtos.Response;
using Domain.Entities;

namespace Application.Interfaces.IMapper
{
    public interface IPaymentStatusEntityMapper
    {
        Task<PaymentStatusResponseDto> MapToResponseDto(PaymentStatusEntity status);
    }
}
