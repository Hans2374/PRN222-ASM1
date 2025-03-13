using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PaymentCVSTS.MVCWebApp.Middleware
{
    public class AuthorizationCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check if this is an AJAX request requiring authentication
            bool isAjaxRequest = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            bool isPaymentsRoute = context.Request.Path.Value?.Contains("/Payments") ?? false;
            bool isApiRoute = context.Request.Path.Value?.Contains("/api/") ?? false;

            if ((isAjaxRequest || isApiRoute) && isPaymentsRoute && !context.User.Identity.IsAuthenticated)
            {
                // Return 401 Unauthorized for AJAX or API requests
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "Authentication required to access this resource",
                    redirectTo = "/UserAccounts/Login"
                });
                return;
            }

            await _next(context);
        }
    }

    // Extension method to add this middleware to the pipeline
    public static class AuthorizationCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorizationCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthorizationCheckMiddleware>();
        }
    }
}