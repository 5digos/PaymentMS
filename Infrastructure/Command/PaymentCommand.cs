using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.ICommand;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Command 
{
    class PaymentCommand : IPaymentCommand  
    {  
        private readonly AppDbContext _context;

        public PaymentCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreatePaymentAsync (Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment.PaymentId;
        }

        public async Task DeletePaymentAsync(Guid paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Payment not found");
            }
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentStatusAsync(Payment payment, int status)
        {
            payment.PaymentStatusId = status;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
