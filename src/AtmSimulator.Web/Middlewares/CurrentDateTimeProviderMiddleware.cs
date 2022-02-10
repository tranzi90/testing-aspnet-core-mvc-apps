using System.Threading.Tasks;
using AtmSimulator.Web.Models.Domain;
using Microsoft.AspNetCore.Http;

namespace AtmSimulator.Web.Middlewares
{
    public class CurrentDateTimeProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CurrentDateTimeProviderMiddleware(
            RequestDelegate next,
            IDateTimeProvider dateTimeProvider)
        {
            _next = next;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/current-unix-time-seconds"))
            {
                var currentUnixTimeSeconds = _dateTimeProvider.UtcNow.ToUnixTimeSeconds().ToString();

                await context.Response.WriteAsync(currentUnixTimeSeconds);

                return;
            }

            await _next(context);
        }
    }
}
