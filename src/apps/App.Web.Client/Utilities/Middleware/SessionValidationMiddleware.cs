using App.Web.Client.Extensions;
using App.Web.Client.Services.Abstractions;
using Microsoft.AspNetCore.Authentication;

namespace App.Web.Client.Utilities.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IUserSessionService sessionService)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.GetIdentifier();
                var sessionId = context.User.GetSessionId();

                if (!await sessionService.IsSessionExistsAsync(userId, sessionId))
                {
                    await context.SignOutAsync(); // Logout
                    context.Response.Redirect("/Account/Login"); // Or show warning
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class SessionValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionValidationMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionValidationMiddleware>();
        }
    }
}
