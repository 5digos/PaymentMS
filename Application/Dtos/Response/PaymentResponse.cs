using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dtos.Response
{
    public class PaymentResponse
    {
        public Guid PaymentId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public int PaymentStatusId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string Status { get; set; }
    }
}
