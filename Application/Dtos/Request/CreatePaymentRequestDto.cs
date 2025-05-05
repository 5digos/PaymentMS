using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dtos.Request
{
    public class CreatePaymentRequestDto
    {
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } // Propiedad añadida
        public string Description { get; set; }
        public string CallbackUrl { get; set; }
        public string NotificationUrl { get; set; }
        public PayerInfoDto Payer { get; set; }
        public Dictionary<string, object> Metadata { get; set; } // Propiedad añadida para soportar metadata
    }

    public class PayerInfoDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Identification { get; set; }
        public string IdentificationType { get; set; }
    }
}
