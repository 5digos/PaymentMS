using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeders
{
    public static class PaymentMethodSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentMethod>().HasData(
                new PaymentMethod { Id = 1, Name = "Mercado Pago" }
            );
        }
        
    }
}
