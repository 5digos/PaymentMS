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
        public PaymentQuery(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatusEntity)
                .ToListAsync();
        }
        public async Task<Payment> GetPaymentByIdAsync(Guid id)
        {
            return await _context.Payments
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatusEntity)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Payment> GetPaymentByReservationIdAsync(Guid reservationId)
        {
            return await _context.Payments
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatusEntity)
                .FirstOrDefaultAsync(p => p.ReservationId == reservationId);
        }
        public async Task<Payment> GetPaymentByReferenceAsync(string reference)
        {
            return await _context.Payments
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatusEntity)
                .FirstOrDefaultAsync(p => p.Reference == reference);
        }
        public async Task<List<Payment>> GetPaymentsByMethodIdAsync(int methodId)
        {
            return await _context.Payments
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatusEntity)
                .Where(p => p.PaymentMethodId == methodId)
                .ToListAsync();
        }
        public async Task<List<Payment>> GetPaymentsByStatusIdAsync(int statusId)
        {
            return await _context.Payments
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatusEntity)
                .Where(p => p.PaymentStatusId == statusId)
                .ToListAsync();
        }
    }
}
