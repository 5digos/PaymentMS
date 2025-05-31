using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data
{
    public class PaymentMethodData : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Name).IsRequired().HasMaxLength(50);
            builder.Property(pm => pm.Description).HasMaxLength(255);
            builder.Property(pm => pm.IsActive).IsRequired();

            builder.HasMany(pm => pm.Payments)
                .WithOne(p => p.PaymentMethod)
                .HasForeignKey(p => p.PaymentMethodId);

            builder.HasData(
                new PaymentMethod { Id = 1, Name = "MercadoPago", Description = "Pago a través de MercadoPago", IsActive = true },
                new PaymentMethod { Id = 2, Name = "Transferencia Bancaria", Description = "Pago mediante transferencia bancaria", IsActive = true },
                new PaymentMethod { Id = 3, Name = "Efectivo", Description = "Pago en efectivo", IsActive = true }
            );
        }
    }
}
