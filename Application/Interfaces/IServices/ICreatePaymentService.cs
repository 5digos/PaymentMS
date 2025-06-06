﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Domain.Entities;

namespace Application.Interfaces.IServices
{
    public interface ICreatePaymentService
    {
        Task<Guid> CreatePayment(CreatePaymentRequestDto paymentResponseDto);
        Task SavePayment(Payment payment );
    } 
}
