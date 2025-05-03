using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Interfaces; 
using System.Reflection.Metadata;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Query
{
    public class PaymentQuery : IPaymentQuery 
    {
        private readonly AppDbContext _context;

        public PaymentQuery (AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            var payments = _context.Payments.ToListAsync();
            return await payments;
        }

        public async Task<Payment?> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.ReservationId)
                .Include(p => p.PaymentMethodId)
                .Include(p => p.PaymentStatus)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                return null;
            }
            return payment;
        }

        public async Task<List<Payment>> GetPaymentsByDateAsync(DateTime date)
        {
            var payments = await _context.Payments
                .Include(p => p.ReservationId)
                .Include(p => p.PaymentMethodId)
                .Include(p => p.PaymentStatus)
                .Where(p => p.Date == date)
                .ToListAsync();

            return payments;
        }

        public async Task<Payment?> GetPaymentByReservationIdAsync(Guid reservationId)
        {
            var payment = await _context.Payments
                .Include(p => p.ReservationId)
                .Include(p => p.PaymentMethodId)
                .Include(p => p.PaymentStatus)
                .FirstOrDefaultAsync(p => p.ReservationId == reservationId);

            return payment;
        }

        public async Task<List<Payment>> GetPaymentsByStatusIdAsync(int status)
        {
            var payments = await _context.Payments
                .Include(p => p.ReservationId)
                .Include(p => p.PaymentMethodId)
                .Include(p => p.PaymentStatus)
                .Where(p => p.PaymentStatusId == status)
                .ToListAsync();

            return payments;
        }

        public async Task<List<Payment>> GetPaymentsByMethodIdAsync(int paymentMethodId)
        {
            var payments = await _context.Payments
                .Include(p => p.ReservationId)
                .Include(p => p.PaymentMethodId)
                .Include(p => p.PaymentStatus)
                .Where(p => p.PaymentMethodId == paymentMethodId)
                .ToListAsync();

            return payments;
        }
    }
}
