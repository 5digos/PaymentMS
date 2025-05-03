using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Interfaces.ICommand;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.UseCase.Payments
{
    public class CreatePaymentService : ICreatePaymentService 
    {
        private readonly IPaymentCommand _paymentCommand;
        public CreatePaymentService(IPaymentCommand paymentCommand)
        {
            _paymentCommand = paymentCommand;
        }

        public async Task<Guid> CreatePayment(CreatePaymentRequestDto request)
        {
            return await _paymentCommand.CreatePaymentAsync(request.ReservationId, request.Amount, request.PaymentMethodId);
        }   
    }
}
