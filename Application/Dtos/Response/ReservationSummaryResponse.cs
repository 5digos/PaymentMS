using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Response
{
    public class ReservationSummaryResponse 
    {
        public Guid ReservationId { get; set; }
        public int UserId { get; set; }
        public Guid VehicleId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Tiempos reales de pickup/return
        public DateTime? ActualPickupTime { get; set; }
        public DateTime? ActualReturnTime { get; set; }

        // Snapshot de tarifa y costos posteriores
        public decimal? HourlyRateSnapshot { get; set; }
    }
}
