using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");
                entity.HasKey(p => p.PaymentId);
                entity.Property(p => p.Date).IsRequired();
                entity.Property(p => p.Amount).HasColumnType("varchar(255)").IsRequired();
                //relacion con Reservation
                /*
                 */
                //relacion con PaymentMethod
                entity.HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.Payments)
                .HasForeignKey(p => p.PaymentMethodId).IsRequired();
                //.OnDelete(DeleteBehavior.Restrict);
                //relacion con PaymentStatus
                entity.HasOne(p => p.PaymentStatus)
                .WithMany(ps => ps.Payments)
                .HasForeignKey(p => p.PaymentStatusId).IsRequired();
                //.OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("PaymentMethod");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).HasColumnType("varchar(25)").IsRequired();
            });

            modelBuilder.Entity<PaymentStatus>(entity =>
            {
                entity.ToTable("PaymentStatus");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).HasColumnType("varchar(25)").IsRequired();
            });
        }
    }
}
