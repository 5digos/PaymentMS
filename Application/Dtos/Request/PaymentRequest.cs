using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dtos.Request
{
    public class PaymentRequest
    {
        public Guid ReservationId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        
    }
}
