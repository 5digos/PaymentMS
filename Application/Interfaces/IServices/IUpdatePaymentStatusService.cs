using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Request;

namespace Application.Interfaces.IServices
{
    public interface IUpdatePaymentStatusService
    {
        Task<bool> UpdatePaymentStatus(UpdatePaymentStatusRequestDto updatePaymentStatusRequestDto);
    }
}
