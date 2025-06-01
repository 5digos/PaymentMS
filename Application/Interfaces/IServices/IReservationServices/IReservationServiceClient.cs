using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Request;
using Application.Dtos.Response;

namespace Application.Interfaces.IServices.IReservationServices
{
    public interface IReservationServiceClient
    {
        Task<ReservationSummaryResponse> GetReservationAsync(Guid id);
        Task ConfirmPaymentAsync(Guid reservationId, PaymentConfirmationRequest request);
    }
}
