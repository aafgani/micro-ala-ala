using System.Security.Claims;
using App.Common.Domain.Auth;

namespace App.Web.Client.Extensions;

public static class CustomAuthorization
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
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
        });

        return services;
    }
}
