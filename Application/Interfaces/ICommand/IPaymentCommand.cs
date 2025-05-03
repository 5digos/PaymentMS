using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.ICommand
{
    public interface IPaymentCommand 
    {
        Task<Guid> CreatePaymentAsync(Payment payment); 

        Task AddPaymentAsync(Payment payment);

        Task DeletePaymentAsync(Guid paymentId);

        Task UpdatePaymentStatusAsync(Payment payment, int status);
    }

}
