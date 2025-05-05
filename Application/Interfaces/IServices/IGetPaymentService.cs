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
        Task<List<Payment>> GetAllPayments();
        Task<PaymentResponseDto> GetPaymentById(Guid id);
        Task<PaymentResponseDto> GetPaymentByIdAsync(Guid id);
        Task<Payment> GetPaymentByReservationId(Guid reservationId);
        Task<Payment> GetPaymentByReferenceAsync(string reference);
        Task<List<Payment>> GetPaymentsByMethodId(int methodId);
        Task<List<Payment>> GetPaymentsByStatusId(int statusId);
    }
}
