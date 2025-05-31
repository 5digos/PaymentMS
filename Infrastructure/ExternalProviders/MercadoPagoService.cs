
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using System.Text.Json;
using Infrastructure.HttpClients.Dtos;
using RestSharp;

public class MercadoPagoService
{
    public MercadoPagoService(string accessToken)
    {
        MercadoPagoConfig.AccessToken = accessToken;
    }

    public async Task<string> CreatePreferenceAsync(string title, decimal amount)
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
                Success = "https://tuweb.com/pago-exitoso",
                Failure = "https://tuweb.com/pago-fallido",
                Pending = "https://tuweb.com/pago-pendiente"
            },
            AutoReturn = "approved"
        };

        Preference preference = await client.CreateAsync(request);
        return preference.InitPoint; // Este es el link que redirige al Checkout Pro
    }

    public async Task<MercadoPagoPaymentInfo> GetPaymentInfoAsync(long paymentId) 
    {
        var client = new RestClient("https://api.mercadopago.com"); 
        var request = new RestRequest($"/v1/payments/{paymentId}", Method.Get);
        request.AddHeader("Authorization", "Bearer TU_ACCESS_TOKEN");

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
            throw new Exception("No se pudo obtener el pago");

        var json = JsonDocument.Parse(response.Content!);
        var status = json.RootElement.GetProperty("status").GetString();
        var transactionId = json.RootElement.GetProperty("id").GetInt64().ToString();

        return new MercadoPagoPaymentInfo
        {
            Status = status!,
            TransactionId = transactionId
        };
    }

}
