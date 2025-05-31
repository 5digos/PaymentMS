using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Response;
using Domain.Entities;

namespace Application.Interfaces.IServices
{
    public interface IGetPaymentService
    {
        Task<PaymentResponseDto> GetPaymentResponseDtoById(Guid id);
        Task<Payment?> GetPaymentByReservationId(Guid reservationId);
        Task<List<Payment>> GetPaymentsByStatusId(int statusId);
        Task<List<Payment>> GetPaymentsByMethodId(int methodId);
        Task<List<Payment>> GetAllPayments();
        Task<Payment?> GetPaymentByIdAsync(Guid id);

    }
}
