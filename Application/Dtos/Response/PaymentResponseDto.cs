using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dtos.Response
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public string Reference { get; set; }
        public string ExternalId { get; set; } // Propiedad añadida
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string CheckoutUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
