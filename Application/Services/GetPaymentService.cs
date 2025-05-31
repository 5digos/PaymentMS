using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dtos.Response;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.Services
{
    public class GetPaymentService : IGetPaymentService
    {
        private readonly IPaymentQuery _paymentQuery;

        public GetPaymentService(IPaymentQuery paymentQuery)
        {
            _paymentQuery = paymentQuery;
        }

        public async Task<List<Payment>> GetAllPayments()
        {
            return await _paymentQuery.GetAllPaymentsAsync();
        }

        public async Task<PaymentResponseDto> GetPaymentById(Guid id)
        {
            return await GetPaymentByIdAsync(id);
        }

        public async Task<PaymentResponseDto> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _paymentQuery.GetPaymentByIdAsync(id);

            if (payment == null)
            {
                return null;
            }

            return new PaymentResponseDto
            {
                Id = payment.Id,
                Reference = payment.Reference,
                ExternalId = payment.ExternalId,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                CheckoutUrl = payment.CheckoutUrl,
                CreatedAt = payment.CreatedAt
            };
        }

        public async Task<Payment> GetPaymentByReservationId(Guid reservationId)
        {
            return await _paymentQuery.GetPaymentByReservationIdAsync(reservationId);
        }

        public async Task<Payment> GetPaymentByReferenceAsync(string reference)
        {
            return await _paymentQuery.GetPaymentByReferenceAsync(reference);
        }

        public async Task<List<Payment>> GetPaymentsByMethodId(int methodId)
        {
            return await _paymentQuery.GetPaymentsByMethodIdAsync(methodId);
        }

        public async Task<List<Payment>> GetPaymentsByStatusId(int statusId)
        {
            return await _paymentQuery.GetPaymentsByStatusIdAsync(statusId);
        }
    }
}