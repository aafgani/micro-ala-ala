using App.Common.Infrastructure.Cache;
using App.Web.UI.Utilities.Session;

namespace App.Web.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInternalServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<ICacheService, CacheService>();
        services.AddScoped<IUserSessionService, UserSessionService>();
        services.AddHttpContextAccessor();

        return services;
    }

}
