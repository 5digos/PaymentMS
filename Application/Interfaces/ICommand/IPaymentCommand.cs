using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Request;
using Domain.Entities;

namespace Application.Interfaces.ICommand
{
    public interface IPaymentCommand 
    {
        Task<Guid> CreatePaymentAsync(Guid reservationId, decimal amount, int paymentMethodId);

        Task AddPaymentAsync(Payment payment);

        Task DeletePaymentAsync(Guid paymentId);

        Task<bool> UpdatePaymentStatusAsync(Guid paymentId, int newstatusId);
    }

}
