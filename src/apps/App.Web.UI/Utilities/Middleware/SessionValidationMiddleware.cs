using App.Web.UI.Extensions;
using Microsoft.AspNetCore.Authentication;

namespace App.Web.UI.Utilities.Session;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;

    public SessionValidationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IUserSessionService sessionService)
    {
        // Ensures only users with a valid session can access protected resources.
        // Prevents access if the session is expired or invalidated elsewhere.
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