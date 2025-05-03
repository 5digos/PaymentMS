using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IUpdatePaymentStatusService
    {
        Task UpdatePaymentStatus(Guid paymentId, int status);
    }
}
