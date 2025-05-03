using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Payment> Payments { get; set; }
    }
}