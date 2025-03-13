using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PaymentCVSTS.MVCWebApp.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request processing");

                // Store the exception in TempData so it can be accessed by the error view
                if (context.Items.ContainsKey("Exception"))
                {
                    context.Items["Exception"] = ex;
                }
                else
                {
                    context.Items.Add("Exception", ex);
                }

                // Redirect to error page
                context.Response.Redirect("/Home/Error");
            }
        }
    }

    // Extension method to easily add this middleware to the pipeline
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}