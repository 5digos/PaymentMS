using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeders
{
    public static class PaymentStatusSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentStatus>().HasData(
                    new PaymentStatus { Id = 1, Name = "Pendiente" },
                    new PaymentStatus { Id = 2, Name = "Aceptado" },
                    new PaymentStatus { Id = 3, Name = "Rechazado" }
                );
        }
    }
}
