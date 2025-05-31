using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validators
{
    public class PaymentRequestValidator : AbstractValidator<CreatePaymentRequestDto>
    {
        public PaymentRequestValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto debe ser mayor que cero");
            RuleFor(x => x.Currency).NotEmpty().WithMessage("La moneda es requerida");
            RuleFor(x => x.Description).NotEmpty().WithMessage("La descripción es requerida");
            RuleFor(x => x.Reference).NotEmpty().WithMessage("La referencia es requerida");
            RuleFor(x => x.CallbackUrl).NotEmpty().WithMessage("La URL de retorno es requerida")
                                      .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                                      .WithMessage("La URL de retorno debe ser una URL válida");
            RuleFor(x => x.NotificationUrl).NotEmpty().WithMessage("La URL de notificación es requerida")
                                         .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                                         .WithMessage("La URL de notificación debe ser una URL válida");

            // Validar información del pagador si se proporciona
            When(x => x.Payer != null && !string.IsNullOrEmpty(x.Payer.Email), () => {
                RuleFor(x => x.Payer.Email).EmailAddress().WithMessage("El email del pagador debe ser una dirección válida");
            });
        }
    }
}
