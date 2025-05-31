using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HttpClients.Dtos
{
    public class MercadoPagoPaymentInfo
    {
        public string Status { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
    }

}
