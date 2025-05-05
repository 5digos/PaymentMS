using System; 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Request;
using Application.Interfaces.ICommand;
using Azure.Core;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Command 
{
    public class PaymentCommand : IPaymentCommand  
    {  
        private readonly AppDbContext _context;

        public PaymentCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreatePaymentAsync (Guid reservationId, decimal amount, int paymentMethodId)
        {
            var payment = new Payment 
            {
                PaymentId = Guid.NewGuid(),
                ReservationId = reservationId,
                Amount = amount,
                Date = DateTime.UtcNow,
                PaymentStatusId = 1, // Pendiente
                PaymentMethodId = paymentMethodId,
            };
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

        public async Task<bool> UpdatePaymentStatusAsync(Guid paymentId, int newStatusId) 
        {
            Console.WriteLine($"Buscando PaymentId: {paymentId}");

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                Console.WriteLine("No se encontró el pago con ese ID.");

                return false;
            }

            payment.PaymentStatusId = newStatusId;
            await _context.SaveChangesAsync();

            return true;

        }
    }
}
