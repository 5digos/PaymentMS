using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PaymentStatus
    {
        public int Id { get; set; } // 1 - Pending, 2 - Aprobado, 3 - Rechazado
        public string Name { get; set; }
        public IList<Payment> Payments { get; set; }
    }
}