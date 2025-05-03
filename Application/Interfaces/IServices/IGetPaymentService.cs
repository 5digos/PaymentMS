using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IServices
{
    public interface IGetPaymentService
    {
        Task<Payment> GetPaymentByIdAsync(Guid id);
        Task<List<Payment>> GetPaymentByReservationIdAsync(Guid reservationId);
        Task<List<Payment>> GetPaymentsByStatusIdAsync(int statusId);
        Task<List<Payment>> GetPaymentsByMethodIdAsync(int methodId);
        Task<List<Payment>> GetAllPaymentsAsync();

    }
}
