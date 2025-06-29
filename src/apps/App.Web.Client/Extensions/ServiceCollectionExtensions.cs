using App.Common.Infrastructure.Cache;
using App.Web.Client.Services.Abstractions;
using App.Web.Client.Services.Implementation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net;
using System.Security.Claims;

namespace App.Web.Client.Extensions
{
    public static class ServiceCollectionExtensions
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
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
                options.Cookie.SameSite = SameSiteMode.Strict; // or Lax
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });

            return services;
        }

        public static IServiceCollection AddInternalServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IUserSessionService, UserSessionService>();
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

            var identity = (ClaimsIdentity)context.Principal.Identity;
            identity.AddClaim(new Claim("session_id", sessionId));
            await sessionService.SetUserSessionAsync(userId, sessionId, CancellationToken.None);

            await Task.CompletedTask.ConfigureAwait(false);
        }
        #endregion
    }
}
