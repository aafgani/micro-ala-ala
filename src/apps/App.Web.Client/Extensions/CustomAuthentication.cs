using System.Net;
using System.Security.Claims;
using App.Common.Domain.Auth;
using App.Common.Domain.Configuration;
using App.Web.Client.Services.Abstractions;
using App.Web.Client.Utilities.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace App.Web.Client.Extensions;

public static class CustomAuthentication
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration config)
    {
        // services.AddMicrosoftIdentityWebAppAuthentication(config, "AzureEntra")
        //     .EnableTokenAcquisitionToCallDownstreamApi(scopes)
        //     .AddInMemoryTokenCaches();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               options.LoginPath = "/Account/Login"; // Optional override
               options.AccessDeniedPath = "/Account/AccessDenied"; // Optional
           })
           .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
           {
               config.Bind("AzureEntra", options);

               // ✅ Use authorization code flow with PKCE
               options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
               options.UsePkce = true;
               options.SaveTokens = true; // Required for token acquisition
               options.GetClaimsFromUserInfoEndpoint = false;

               // ✅ Custom events
               options.Events = new OpenIdConnectEvents
               {
                   OnRemoteFailure = OnRemoteFailureFunc,
                   OnAuthorizationCodeReceived = OnAuthorizationCodeReceivedFunc,
                   OnTokenValidated = OnTokenValidatedFunc
               };

               options.Scope.Add("openid");
               options.Scope.Add("profile");
               options.Scope.Add("email");
           });

        // ✅ Configure OpenIdConnect events AFTER Microsoft.Identity.Web setup
        services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnRemoteFailure = OnRemoteFailureFunc;
            options.Events.OnAuthorizationCodeReceived = OnAuthorizationCodeReceivedFunc;
            options.Events.OnTokenValidated = OnTokenValidatedFunc;
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

        // ✅ Register TokenService for token acquisition
        services.AddScoped<ITokenService, TokenService>();

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
        try
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<TokenService>>();

            var scopes = config["TodoApi:Scopes"]?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            var app = ConfidentialClientApplicationBuilder
                .Create(config["AzureEntra:ClientId"])
                .WithClientSecret(config["AzureEntra:ClientSecret"])
                .WithRedirectUri(config["AzureEntra:RedirectUri"])
                .WithAuthority(new Uri(config["AzureEntra:Authority"]))
                .Build();

            app.AddInMemoryTokenCache();

            var result = await app.AcquireTokenByAuthorizationCode(scopes, context.ProtocolMessage.Code).ExecuteAsync();

            var accountId = result.Account.HomeAccountId.Identifier;
            if (context.Principal?.Identity is ClaimsIdentity identity)
            {
                identity.AddClaim(new Claim("msal_account_id", accountId));
            }

            logger.LogInformation("Successfully acquired token and added account {AccountId} to MSAL cache", accountId);

            // Save token or account ID to session/cookie/db if needed
            context.HandleCodeRedemption(result.AccessToken, result.IdToken);
        }
        catch (Exception ex)
        {
            // ✅ Log the error and handle gracefully
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ITokenAcquisition>>();
            logger.LogError(ex, "Failed to acquire access token during authorization code received event");

            throw new Exception("Failed to acquire access token during authorization code received event", ex);
        }
    }

    private static async Task OnTokenValidatedFunc(TokenValidatedContext context)
    {
        // TODO: Replace hardcoded roles with roles retrieved from User API.
        //       Add logic to call User API and enrich claims with user-specific roles and info.

        if (context.Principal?.Identity is not ClaimsIdentity identity)
        {
            context.Fail("Invalid principal or identity");
            return;
        }

        var userId = context.Principal.GetNameIdentifierId();
        if (string.IsNullOrEmpty(userId))
        {
            context.Fail("User ID not found in claims");
            return;
        }

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
