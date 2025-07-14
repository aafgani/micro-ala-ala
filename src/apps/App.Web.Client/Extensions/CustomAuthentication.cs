using System.Net;
using System.Security.Claims;
using App.Common.Domain.Auth;
using App.Web.Client.Services.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace App.Web.Client.Extensions;

public static class CustomAuthentication
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               options.LoginPath = "/Account/Login"; // Optional override
               options.AccessDeniedPath = "/Account/AccessDenied"; // Optional
           })
           .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
           {
               config.Bind("AzureEntra", options);
               options.ResponseType = OpenIdConnectResponseType.Code;
               options.Events ??= new OpenIdConnectEvents();
               options.Events.OnTokenValidated += OnTokenValidatedFunc;
               options.Events.OnAuthorizationCodeReceived += OnAuthorizationCodeReceivedFunc;
               options.Events.OnRemoteFailure += OnRemoteFailureFunc;
           });

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "AppWebClientAuth"; // Custom cookie name
            options.Cookie.HttpOnly = true; // Prevents JavaScript access
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use HTTPS
            options.Cookie.SameSite = SameSiteMode.Strict; // Prevents CSRF attacks
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session expiration
            options.SlidingExpiration = true; // Resets expiration on each request
        });
        return services;
    }

    #region private
    private static Task OnRemoteFailureFunc(RemoteFailureContext context)
    {
        var errorMessage = context.Failure?.Message ?? "Login failed.";
        var redirectUri = $"/error?statusCode={HttpStatusCode.Forbidden}&message={Uri.EscapeDataString(errorMessage)}";

        context.Response.Redirect(redirectUri);
        context.HandleResponse(); // stop the exception from bubbling
        return Task.CompletedTask;
    }

    private static async Task OnAuthorizationCodeReceivedFunc(AuthorizationCodeReceivedContext context)
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }

    private static async Task OnTokenValidatedFunc(TokenValidatedContext context)
    {
        // TODO: Replace hardcoded roles with roles retrieved from User API.
        //       Add logic to call User API and enrich claims with user-specific roles and info.

        var userId = context.Principal.GetNameIdentifierId();
        var sessionId = Guid.NewGuid().ToString();
        var sessionService = context.HttpContext.RequestServices.GetRequiredService<IUserSessionService>();

        var isExists = await sessionService.IsSessionExistsAsync(userId, sessionId);

        if (isExists)
        {
            context.HttpContext.Items["AuthError"] = "Another session already exists.";
            context.Fail("Another session already exists.");
            return;
        }

        await sessionService.SetUserSessionAsync(userId, sessionId, CancellationToken.None);

        // Example roles, replace with actual API call
        var userRolesFromApi = new[] {
            Roles.Admin,
            Roles.TodosUser,
            Roles.TodosAdmin,
            Roles.FinanceAdmin
      };

        var identity = (ClaimsIdentity)context.Principal.Identity;
        identity.AddClaim(new Claim("session_id", sessionId));
        identity.AddClaim(new Claim("session_created_at", DateTime.UtcNow.ToString("o"))); // ISO 8601 format
        identity.AddClaim(new Claim("session_expires_at", DateTime.UtcNow.AddMinutes(30).ToString("o"))); // 30 minutes expiration

        foreach (var role in userRolesFromApi)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    #endregion
}
