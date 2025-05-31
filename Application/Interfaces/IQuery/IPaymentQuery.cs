using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public interface IPaymentQuery
    {
        Task<List<Payment>> GetAllPaymentsAsync();
        Task<Payment> GetPaymentByIdAsync(Guid id);
        Task<Payment> GetPaymentByReservationIdAsync(Guid reservationId);
        Task<Payment> GetPaymentByReferenceAsync(string reference);
        Task<List<Payment>> GetPaymentsByMethodIdAsync(int methodId);
        Task<List<Payment>> GetPaymentsByStatusIdAsync(int statusId);
    }
}
