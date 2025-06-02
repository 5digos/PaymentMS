using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using System.Text.Json;
using Infrastructure.HttpClients.Dtos;
using RestSharp;
using MercadoPago.Client.Payment;
using Microsoft.Extensions.Configuration;

public class MercadoPagoService
{
    private readonly string _accessToken;
    private readonly string _backUrlBase;

    public MercadoPagoService(IConfiguration configuration)
    {
        _accessToken = configuration["MercadoPago:AccessToken"];
        _backUrlBase = configuration["MercadoPago:BackUrlBase"];
        MercadoPagoConfig.AccessToken = _accessToken;
    }

    public async Task<string> CreatePreferenceAsync(string title, decimal amount, Guid paymentId, decimal lateFee)
    {
        var client = new PreferenceClient();

        var referenceData = new PaymentReferenceData
        {
            PaymentId = paymentId,
            LateFee = lateFee
        };

        //guardo en externalReference el paymentId y el lateFee
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
            {
                Success = "https://localhost:7059/front/success.html",
                Failure = "https://localhost:7059/front/failure.html",
                Pending = "https://localhost:7059/front/pending.html"
            },
            AutoReturn = "approved",
            ExternalReference = externalReference
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
