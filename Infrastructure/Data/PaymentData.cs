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
    public class PaymentData : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.ExternalId).HasMaxLength(100);
            builder.Property(p => p.Amount).IsRequired();
            builder.Property(p => p.Currency).HasMaxLength(10);
            builder.Property(p => p.Description).HasMaxLength(255);
            builder.Property(p => p.Reference).HasMaxLength(50);
            builder.Property(p => p.CheckoutUrl).HasMaxLength(255);

            builder.HasOne(p => p.PaymentStatusEntity)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.PaymentStatusId);

            builder.HasOne(p => p.PaymentMethod)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.PaymentMethodId);
        }
    }
}
