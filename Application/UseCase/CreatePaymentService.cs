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
        private readonly IPaymentQuery _paymentQuery;

        public CreatePaymentService(IPaymentCommand paymentCommand, IPaymentQuery paymentQuery)
        {
            _paymentCommand = paymentCommand;
            _paymentQuery = paymentQuery;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
        {
            var payment = new Payment
            {
                ReservationId = request.ReservationId,
                Amount = request.Amount,
                PaymentMethodId = request.PaymentMethodId,
                PaymentStatusId = 1 // 1 = Pendiente
            };

            var createdPaymentId= await _paymentCommand.CreatePaymentAsync(payment);

            var paymentCreated = await _paymentQuery.GetPaymentById(createdPaymentId);

            // devolvemos DTO
            return new PaymentResponse
            {
                PaymentId = paymentCreated.PaymentId,
                Amount = paymentCreated.Amount,
                Status = "Pending"
            };
        }   
    }
}
