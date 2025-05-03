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

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");

                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.PaymentId).HasColumnType("uniqueidentifier").ValueGeneratedOnAdd();

                entity.Property(e => e.ReservationId).HasColumnType("uniqueidentifier");

                entity.Property(e => e.Date).HasColumnType("datetime");
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.PaymentMethod)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(e => e.PaymentMethodId);

                entity.HasOne(e => e.PaymentStatus)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(e => e.PaymentStatusId);

            });

            modelBuilder.Entity<PaymentMethod>(entity =>

            {
                entity.ToTable("PaymentMethods");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasColumnType("nvarchar(25)");

            });

            modelBuilder.Entity<PaymentStatus>(entity =>

            {
                entity.ToTable("PaymentStatus");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasColumnType("nvarchar(25)");

            });
        }
    }
}
