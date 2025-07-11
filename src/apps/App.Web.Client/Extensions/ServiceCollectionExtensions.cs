using App.Common.Infrastructure.Cache;
using App.Web.Client.Services.Abstractions;
using App.Web.Client.Services.Implementation;

namespace App.Web.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInternalServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IUserSessionService, UserSessionService>();
            services.AddHttpClient();
            return services;
        }
    }
}
