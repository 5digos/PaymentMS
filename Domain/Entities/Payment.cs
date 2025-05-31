using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public PaymentStatus Status { get; set; }
        public int PaymentStatusId { get; set; }
        public PaymentStatusEntity PaymentStatusEntity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Reference { get; set; }
        public string CheckoutUrl { get; set; }
        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Guid ReservationId { get; set; }

        public Payment() { }

        public Payment(string externalId, decimal amount, string currency, string description)
        {
            Id = Guid.NewGuid();
            ExternalId = externalId;
            Amount = amount;
            Currency = currency;
            Description = description;
            Status = PaymentStatus.Pending;
            PaymentStatusId = (int)PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            Reference = $"PAY-{Id.ToString().Substring(0, 8).ToUpper()}";
        }

        public void SetExternalData(string externalId, string checkoutUrl)
        {
            ExternalId = externalId;
            CheckoutUrl = checkoutUrl;
        }

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
