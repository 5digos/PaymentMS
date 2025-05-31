
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

    public async Task<string> CreatePreferenceAsync(string title, decimal amount, Guid paymentId)
    {
        var client = new PreferenceClient();

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
            {
                Success = "https://tuweb.com/pago-exitoso", //de la url hay que sacar el paymentId para poder usarlo en el [HttpPost("verify/{mercadoPagoPaymentId:long}")]
                Failure = "https://tuweb.com/pago-fallido",
                Pending = "https://tuweb.com/pago-pendiente"
            },
            AutoReturn = "approved",
            //NotificationUrl = "https://localhost:7052/api/payments/notifications",//configurar para que tome la url base
            ExternalReference = paymentId.ToString()
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
