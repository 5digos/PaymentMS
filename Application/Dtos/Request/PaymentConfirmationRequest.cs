using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Request
{
    public class PaymentConfirmationRequest
    {
        public decimal TotalAmount { get; set; }
        public decimal LateFee { get; set; }
        public string PaymentGateway { get; set; }
        public string TransactionId { get; set; }
    }
}
