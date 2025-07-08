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
    public class PaymentMapping : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(e => e.PaymentId);
            builder.Property(e => e.PaymentId).HasColumnType("uniqueidentifier").ValueGeneratedOnAdd();

            builder.Property(e => e.ReservationId).HasColumnType("uniqueidentifier");

            builder.Property(e => e.Date).HasColumnType("datetime");

            builder.Property(e => e.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

            builder.HasOne(e => e.PaymentMethod)
                .WithMany(p => p.Payments)
                .HasForeignKey(e => e.PaymentMethodId);

            builder.HasOne(e => e.PaymentStatus)
                .WithMany(p => p.Payments)
                .HasForeignKey(e => e.PaymentStatusId);
        }
    }
}
