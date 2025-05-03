using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.UseCase
{
    class GetPaymentService : IGetPaymentService  
    {
        private readonly IPaymentQuery _paymentQuery;

        public GetPaymentService(IPaymentQuery paymentQuery)
        {
            _paymentQuery = paymentQuery;
        }

        public Task<List<Payment>> GetAllPaymentsAsync()
        {
            return _paymentQuery.GetAllPayments();
        }

        public Task<Payment> GetPaymentByIdAsync(Guid id)
        { 
            return _paymentQuery.GetPaymentById(id);
        }

        public Task<List<Payment>> GetPaymentByReservationIdAsync(Guid reservationId)
        {
            return _paymentQuery.GetPaymentByReservationId(reservationId);
        }

        public Task<List<Payment>> GetPaymentsByMethodIdAsync(int methodId)
        {
            return _paymentQuery.GetPaymentsByMethodId(methodId); 
        }

        public Task<List<Payment>> GetPaymentsByStatusIdAsync(int statusId)
        {
            return _paymentQuery.GetPaymentsByStatusId(statusId); 
        }
    }
}