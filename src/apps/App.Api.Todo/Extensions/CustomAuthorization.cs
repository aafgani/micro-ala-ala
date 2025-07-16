using Microsoft.AspNetCore.Authorization;

namespace App.Api.Todo.Extensions;

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
