using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class PaymentStatusEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Relación con pagos
        public ICollection<Payment> Payments { get; set; }

        // Constructor vacío para EF Core
        public PaymentStatusEntity() { }

        public PaymentStatusEntity(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        // Método para convertir enum a entidad - Corrección
        public static int FromEnum(PaymentStatus status)
        {
            return (int)status; // La conversión explícita ya está correcta
        }

        // Método para convertir entidad a enum - Corrección
        public PaymentStatus ToEnum()
        {
            return (PaymentStatus)Id; // La conversión explícita ya está correcta
        }
    }
}