using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public interface IPaymentQuery
    {
        Task<Payment> GetPaymentById(Guid paymentId);
        Task<List<Payment>> GetAllPayments();
        Task<List<Payment>> GetPaymentByReservationId(Guid reservationId);
        Task<List<Payment>> GetPaymentsByDate(DateTime date);
        Task<List<Payment>> GetPaymentsByStatus(int status);

    }
}
