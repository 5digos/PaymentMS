using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dtos.Request
{
    public class UpdatePaymentStatusRequestDto
    {
        public Guid PaymentId { get; set; }
        public int NewStatusId { get; set; }
    }
}
