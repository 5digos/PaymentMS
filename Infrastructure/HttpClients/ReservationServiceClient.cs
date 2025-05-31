using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Response;
using Application.Interfaces.IServices.IReservationServices;
using Infrastructure.HttpClients.Dtos;

namespace Infrastructure.HttpClients
{
    public class ReservationServiceClient : IReservationServiceClient
    {
        private readonly HttpClient _httpClient;
        public ReservationServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ReservationSummaryResponse> GetReservationAsync(Guid id)
        {
            //llamo al GET /api/v1/Reservations/{id}
            var reserv = await _httpClient.GetFromJsonAsync<ReservationResponseDto>($"api/v1/Reservations/{id}");
            if (reserv == null || reserv.ReservationId == Guid.Empty)
                throw new InvalidOperationException($"La reserva {id} no existe.");

            return new ReservationSummaryResponse //mapeo a un resumen de la reserva
            {
                ReservationId = reserv.ReservationId,
                UserId = reserv.UserId,
                StartTime = reserv.StartTime,
                EndTime = reserv.EndTime,
                ActualPickupTime = reserv.ActualPickupTime,
                ActualReturnTime = reserv.ActualReturnTime,
                HourlyRateSnapshot = reserv.HourlyRateSnapshot
            };
        }
    }
}
