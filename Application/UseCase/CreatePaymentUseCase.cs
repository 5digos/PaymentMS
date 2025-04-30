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
    public class CreatePaymentUseCase : IPaymentService
    {
        private readonly IPaymentCommand _paymentCommand;

        public CreatePaymentUseCase(IPaymentCommand paymentCommand)
        {
            _paymentCommand = paymentCommand;
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

            // guardamos
            var createdPayment = await _paymentCommand.AddPaymentAsync(payment);

            // devolvemos DTO
            return new PaymentResponse
            {
                PaymentId = createdPayment.PaymentId,
                Amount = createdPayment.Amount,
                Status = "Pending"
            };
        }   
    }
}
