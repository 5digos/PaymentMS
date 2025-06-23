using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HttpClients.Dtos
{
    public class ReservationResponseDto
    {
        public Guid ReservationId { get; set; }
        public int UserId { get; set; }
        public Guid VehicleId { get; set; }

        public int PickupBranchOfficeId { get; set; }
        public string PickupBranchOfficeName { get; set; } = null!;

        public int DropOffBranchOfficeId { get; set; }
        public string DropOffBranchOfficeName { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Tiempos reales de pickup/return
        public DateTime? ActualPickupTime { get; set; }
        public DateTime? ActualReturnTime { get; set; }

        // Snapshot de tarifa y costos posteriores
        public decimal? HourlyRateSnapshot { get; set; }

        //public ReservationStatus Status { get; set; }
    }
}
