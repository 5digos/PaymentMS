using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings
{
    //public class PaymentMapping : IEntityTypeConfiguration<Payment>
    //{
    //    public void Configure(EntityTypeBuilder<Payment> builder)
    //    {
    //        builder.ToTable("Payments");
    //        builder.HasKey(e => e.Id);

    //        // Mapeo básico
    //        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
    //        builder.Property(e => e.Currency).HasMaxLength(3).IsRequired();
    //        builder.Property(e => e.Description).HasMaxLength(255);
    //        builder.Property(e => e.ExternalId).HasMaxLength(50).IsRequired();
    //        builder.Property(e => e.Reference).HasMaxLength(50).IsRequired();
    //        builder.Property(e => e.CheckoutUrl).HasMaxLength(500);

    //        // Mapeo para el estado de pago
    //        // Importante: Mapear Status como un enum y PaymentStatusId como un int
    //        builder.Property(e => e.Status)
    //               .HasConversion<int>(); // Convertir el enum a int

    //        builder.Property(e => e.PaymentStatusId)
    //               .IsRequired(); // Ya es un int, no necesita conversión

    //        // Relación con PaymentStatusEntity
    //        builder.HasOne<PaymentStatusEntity>()
    //               .WithMany(ps => ps.Payments)
    //               .HasForeignKey(p => p.PaymentStatusId)
    //               .IsRequired();
    //    }
    //}
}
