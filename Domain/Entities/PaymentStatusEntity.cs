using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class PaymentStatusEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Payment> Payments { get; set; }

        public PaymentStatusEntity() { }

        public PaymentStatusEntity(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public static int FromEnum(PaymentStatus status) => (int)status;
        public PaymentStatus ToEnum() => (PaymentStatus)Id;
    }
}