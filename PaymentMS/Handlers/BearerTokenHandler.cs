using System.Net.Http.Headers;

namespace PaymentMS.Handlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // 1) Leer el Authorization de la petición entrante
            var incomingAuth = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"]
                .ToString();

            if (!string.IsNullOrEmpty(incomingAuth))
            {
                // 2) Lo asignamos al header de la llamada saliente
                // incomingAuth == "Bearer eyJ…"
                var parts = incomingAuth.Split(' ', 2);
                if (parts.Length == 2 && parts[0].Equals("Bearer", System.StringComparison.OrdinalIgnoreCase))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", parts[1]);
                }
            }

            // 3) Continuar la cadena
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
