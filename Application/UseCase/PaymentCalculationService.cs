using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Response;
using Application.Interfaces.IServices;

namespace Application.UseCase
{
    public class PaymentCalculationService : IPaymentCalculationService 
    {
        public decimal CalculateAmount(ReservationSummaryResponse reservation) 
        {
            if (reservation.HourlyRateSnapshot == null)
                throw new ArgumentException("No se puede calcular el precio sin tarifa.");

            var startTime = reservation.StartTime; //siempre el startTime es el que se seteó en la reserva

            // Si el usuario devolvió más tarde, se toma ese tiempo. Si no, se toma EndTime como se seteó de entrada
            var endTime = (reservation.ActualReturnTime.HasValue && reservation.ActualReturnTime > reservation.EndTime)
                ? reservation.ActualReturnTime.Value
                : reservation.EndTime;

            // Duración total redondeada hacia arriba
            var duration = endTime - startTime;
            var totalHours = Math.Ceiling(duration.TotalHours);

            return (decimal)totalHours * reservation.HourlyRateSnapshot.Value;

        }
    }
}
