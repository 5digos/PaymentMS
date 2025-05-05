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
            modelBuilder.Entity<PaymentStatusEntity>().HasData(
                new PaymentStatusEntity((int)PaymentStatus.Pending, "Pending", "El pago está pendiente"),
                new PaymentStatusEntity((int)PaymentStatus.Completed, "Completed", "El pago se completó exitosamente"),
                new PaymentStatusEntity((int)PaymentStatus.Failed, "Failed", "El pago falló"),
                new PaymentStatusEntity((int)PaymentStatus.Cancelled, "Cancelled", "El pago fue cancelado")
            );
        }
    }
}
