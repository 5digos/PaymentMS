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
    public class PaymentStatusData : IEntityTypeConfiguration<PaymentStatusEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentStatusEntity> builder)
        {
            // Configuración aquí
        }
    }
}
