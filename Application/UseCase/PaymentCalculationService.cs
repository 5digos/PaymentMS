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
        public (decimal TotalAmount, decimal LateFee) CalculateAmount(ReservationSummaryResponse reservation)  
        {
            if (reservation.HourlyRateSnapshot == null)
                throw new ArgumentException("No se puede calcular el precio sin tarifa.");

            var startTime = reservation.StartTime; //siempre el startTime es el que se seteó en la reserva
            var rate = reservation.HourlyRateSnapshot.Value;

            // Si el usuario devolvió más tarde, se toma ese tiempo. Si no, se toma EndTime como se seteó de entrada
            var actualEndTime = (reservation.ActualReturnTime.HasValue && reservation.ActualReturnTime > reservation.EndTime)
                ? reservation.ActualReturnTime.Value
                : reservation.EndTime;

            var totalHours = Math.Ceiling((actualEndTime - startTime).TotalHours);
            var baseHours = Math.Ceiling((reservation.EndTime - startTime).TotalHours);

            var totalAmount = (decimal)totalHours * rate;
            var baseAmount = (decimal)baseHours * rate;
            var lateFee = totalAmount - baseAmount;

            return (totalAmount, lateFee > 0 ? lateFee : 0); 
        }
    }
}
