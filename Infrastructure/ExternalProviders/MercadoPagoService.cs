
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using System.Text.Json;
using Infrastructure.HttpClients.Dtos;
using RestSharp;
using MercadoPago.Client.Payment;

public class MercadoPagoService
{
    private readonly string _accessToken;

    public MercadoPagoService(string accessToken)
    {
        _accessToken = accessToken;
        MercadoPagoConfig.AccessToken = accessToken;
    }

    public async Task<string> CreatePreferenceAsync(string title, decimal amount, Guid paymentId, decimal lateFee)
    {
        var client = new PreferenceClient();

        var referenceData = new PaymentReferenceData
        {
            PaymentId = paymentId,
            LateFee = lateFee
        };//guardo en externalReference el paymentId y el lateFee
        var externalReference = JsonSerializer.Serialize(referenceData); //y lo serializo

        var request = new PreferenceRequest
        {
            Items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = title,
                    Quantity = 1,
                    CurrencyId = "ARS",
                    UnitPrice = amount,
                }
            },
            BackUrls = new PreferenceBackUrlsRequest

            {   //en Succes hay que poner la url dada por ngrok para poder sacar el paymentId y asi poder usarlo en el [HttpPost("verify/{mercadoPagoPaymentId:long}")]
                Success = "https://1ac5-2800-810-47f-234-6c45-be7b-438f-d630.ngrok-free.app/api/payment/pago-exitoso",
                Failure = "https://tuweb.com/pago-fallido",
                Pending = "https://tuweb.com/pago-pendiente"
            },
            AutoReturn = "approved",
            ExternalReference = externalReference 
            //NotificationUrl = "https://localhost:7052/api/payments/notifications",//configurar para que tome la url base
        };

        Preference preference = await client.CreateAsync(request);
        return preference.InitPoint; // Este es el link que redirige al Checkout Pro
    }


    public async Task<MercadoPagoPaymentInfo> GetPaymentInfoAsync(long paymentId)
    {
        var client = new PaymentClient();
        var payment = await client.GetAsync(paymentId);

        return new MercadoPagoPaymentInfo
        {
            Status = payment.Status,
            TransactionId = payment.Id.ToString(),
            ExternalReference = payment.ExternalReference
        };
    }
}
