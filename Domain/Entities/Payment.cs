﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Entities
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public Guid ReservationId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public int PaymentMethodId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int PaymentStatusId { get; set; }

    }
}
