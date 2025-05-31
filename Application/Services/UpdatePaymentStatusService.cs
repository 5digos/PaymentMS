using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.Dtos.Request;
using Application.Interfaces;
using Application.Interfaces.ICommand;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.Services
{
    public class UpdatePaymentStatusService : IUpdatePaymentStatusService
    {
        private readonly IPaymentCommand _paymentCommand;

        public UpdatePaymentStatusService(IPaymentCommand paymentCommand)
        {
            _paymentCommand = paymentCommand;
        }

        public async Task<bool> UpdatePaymentStatus(UpdatePaymentStatusRequestDto updatePaymentStatusRequestDto)
        {
            return await UpdatePaymentStatusAsync(updatePaymentStatusRequestDto);
        }

        public async Task<bool> UpdatePaymentStatusAsync(UpdatePaymentStatusRequestDto updatePaymentStatusRequestDto)
        {
            return await _paymentCommand.UpdatePaymentStatusAsync(updatePaymentStatusRequestDto.PaymentId, updatePaymentStatusRequestDto.newStatusId);
        }
    }
}