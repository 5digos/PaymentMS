using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Entities
{
    public class Payment
    {
        // Otras propiedades...
        public Guid Id { get; private set; }
        public string ExternalId { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public string Description { get; private set; }
        public PaymentStatus Status { get; set; } // Enum
        public int PaymentStatusId { get; set; } // Valor entero para la BD
        public PaymentStatusEntity PaymentStatusEntity { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string Reference { get; private set; }
        public string CheckoutUrl { get; private set; }
        public PaymentMethod PaymentMethod { get; set; }  // Opcional
        //public PaymentStatus PaymentStatus => (PaymentStatus)PaymentStatusId; // solo lectura, útil para mostrar
        public Guid ReservationId { get; set; } // o el tipo que uses}
        public int PaymentMethodId { get; set; }

        // Constructor corregido
        public Payment(string externalId, decimal amount, string currency, string description)
        {
            Id = Guid.NewGuid();
            ExternalId = externalId;
            Amount = amount;
            Currency = currency;
            Description = description;
            Status = PaymentStatus.Pending;
            PaymentStatusId = (int)PaymentStatus.Pending; // Conversión explícita
            CreatedAt = DateTime.UtcNow;
            Reference = $"PAY-{Id.ToString().Substring(0, 8).ToUpper()}";
        }

        public void SetExternalData(string externalId, string checkoutUrl)
        {
            ExternalId = externalId;
            CheckoutUrl = checkoutUrl;
        }

        // Métodos de cambio de estado corregidos
        public void Complete()
        {
            Status = PaymentStatus.Completed;
            PaymentStatusId = (int)PaymentStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Fail()
        {
            Status = PaymentStatus.Failed;
            PaymentStatusId = (int)PaymentStatus.Failed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            Status = PaymentStatus.Cancelled;
            PaymentStatusId = (int)PaymentStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
