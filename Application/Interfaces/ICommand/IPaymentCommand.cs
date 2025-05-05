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
        Task<bool> CreateAsync(Payment payment);
        Task<bool> UpdateAsync(Payment payment);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdatePaymentStatusAsync(Guid paymentId, int statusId);
    }
}
