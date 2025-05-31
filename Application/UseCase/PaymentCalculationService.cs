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
        public decimal CalculateAmount(ReservationSummaryResponse reservation) //tengo que mejorarlo
        {
            if (reservation.HourlyRateSnapshot == null)
                throw new ArgumentException("No se puede calcular el precio sin tarifa.");

            var duration = reservation.EndTime - reservation.StartTime;
            var hours = Math.Ceiling(duration.TotalHours);
            return (decimal)hours * reservation.HourlyRateSnapshot.Value; //redondear para arriba: Math.Ceiling

        }
    }
}
