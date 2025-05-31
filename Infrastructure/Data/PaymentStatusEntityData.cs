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
    public class PaymentStatusEntityData : IEntityTypeConfiguration<PaymentStatusEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentStatusEntity> builder)
        {
            builder.HasKey(ps => ps.Id);

            builder.Property(ps => ps.Name).IsRequired().HasMaxLength(50);
            builder.Property(ps => ps.Description).HasMaxLength(255);

            builder.HasMany(ps => ps.Payments)
                .WithOne(p => p.PaymentStatusEntity)
                .HasForeignKey(p => p.PaymentStatusId);

            builder.HasData(
                new PaymentStatusEntity((int)PaymentStatus.Pending, "Pending", "El pago está pendiente"),
                new PaymentStatusEntity((int)PaymentStatus.Completed, "Completed", "El pago se completó exitosamente"),
                new PaymentStatusEntity((int)PaymentStatus.Failed, "Failed", "El pago falló"),
                new PaymentStatusEntity((int)PaymentStatus.Cancelled, "Cancelled", "El pago fue cancelado")
            );
        }
    }
}
