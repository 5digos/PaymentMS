using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Response;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.UseCase
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

        public async Task<Payment?> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _paymentQuery.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return null;
            }

            return payment;
        }


        public async Task<PaymentResponseDto> GetPaymentResponseDtoById(Guid id)
        {
            var payment = await _paymentQuery.GetPaymentByIdAsync(id);

            if (payment == null)
            {
                return null;
            }

            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                ReservationId = payment.ReservationId,
                Amount = payment.Amount,
                Date = payment.Date,
                PaymentMethodName = payment.PaymentMethod?.Name ?? "",
                PaymentStatusName = payment.PaymentStatus?.Name ?? "",
            };
        }

        public async Task<Payment?> GetPaymentByReservationId(Guid reservationId)
        {
            return await _paymentQuery.GetPaymentByReservationIdAsync(reservationId);
        }

        public async Task<List<Payment>> GetPaymentsByMethodId(int methodId)
        {
            return await _paymentQuery.GetPaymentsByMethodIdAsync(methodId);
        }

        public async Task<List<Payment>> GetPaymentsByStatusId(int statusId)
        {
            return await _paymentQuery.GetPaymentsByStatusIdAsync(statusId);
        }

        public async Task<PaymentResponseDto> GetPaymentDtoByReservationId(Guid id)
        {
            var payment = await _paymentQuery.GetPaymentByReservationIdAsync(id);

            if (payment == null)
            {
                return null;
            }

            return new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                ReservationId = payment.ReservationId,
                Amount = payment.Amount,
                Date = payment.Date,
                PaymentMethodName = payment.PaymentMethod?.Name ?? "",
                PaymentStatusName = payment.PaymentStatus?.Name ?? "",
            };
        }

    }
}