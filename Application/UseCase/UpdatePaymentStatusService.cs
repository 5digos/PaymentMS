using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.Interfaces;
using Application.Interfaces.ICommand;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.UseCase
{
    class UpdatePaymentStatusService : IUpdatePaymentStatusService
    { 
        public readonly IPaymentCommand _paymentCommand;
        public readonly IPaymentQuery _paymentQuery;

        public UpdatePaymentStatusService(IPaymentCommand paymentCommand, IPaymentQuery paymentQuery)
        {
            _paymentCommand = paymentCommand;
            _paymentQuery = paymentQuery;
        }

        public async Task UpdatePaymentStatus(Guid paymentId, int status) 
        {
            if (paymentId == Guid.Empty) 
            {
                throw new ArgumentException("Invalid payment ID.");
            }
            if (status < 0 || status > 2) //  0: Pending, 1: Completed, 2: Failed
            {
                throw new ArgumentException("Invalid payment status.");
            }
            var payment = await _paymentQuery.GetPaymentById(paymentId);
            // Call the command to update the payment status
            await _paymentCommand.UpdatePaymentStatusAsync(payment, status); 
        }
    }
}