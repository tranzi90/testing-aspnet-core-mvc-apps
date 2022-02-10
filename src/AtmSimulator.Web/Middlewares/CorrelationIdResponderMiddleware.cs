using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AtmSimulator.Web.Middlewares
{
    public class CorrelationIdResponderMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdResponderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Request.Headers.TryGetValue("Respond-With-Correlation-Id", out var correlationId))
            {
                context.Response.Headers.Add("Correlation-Id", correlationId);
            }
        }
    }
}
