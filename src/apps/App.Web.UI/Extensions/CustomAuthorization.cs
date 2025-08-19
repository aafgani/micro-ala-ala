using System.Security.Claims;
using App.Common.Domain.Auth;
using Microsoft.AspNetCore.Authorization;

namespace App.Web.UI.Extensions;

public static class CustomAuthorization
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthorization(options =>
         {
             var enableAuthentication = config.GetValue<bool>("EnableAuthentication");
             options.AddPolicy("IsAuthenticated", policy =>
             {
                 if (enableAuthentication)
                 {
                     policy.RequireAuthenticatedUser();
                 }
                 else
                 {
                     policy.RequireAssertion(_ => true); // No auth required
                 }
                 // policy.RequireAssertion(context => context.User.Identity?.IsAuthenticated != true);
             });

             options.AddPolicy(Policy.AuthenticatedUser, policy =>
             {
                 policy.RequireAuthenticatedUser();
                 policy.RequireClaim(ClaimTypes.NameIdentifier);
             });

             options.AddPolicy(Policy.Todos, policy =>
             {
                 policy.RequireAuthenticatedUser();
                 policy.RequireRole(Roles.TodosUser);
             });

             options.AddPolicy(Policy.TodosDashboard, policy =>
             {
                 policy.RequireAuthenticatedUser();
                 policy.RequireRole(Roles.TodosContributor);
             });


             // ✅ Set default policy for all endpoints
             options.DefaultPolicy = options.GetPolicy("IsAuthenticated") ?? new AuthorizationPolicyBuilder()
                 .RequireAuthenticatedUser()
                 .Build();

             // ✅ Fallback policy for endpoints without explicit authorization
             options.FallbackPolicy = options.GetPolicy("IsAuthenticated") ?? new AuthorizationPolicyBuilder()
                 .RequireAuthenticatedUser()
                 .Build();
         });

        return services;
    }
}
