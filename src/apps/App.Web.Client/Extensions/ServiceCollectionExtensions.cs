using App.Common.Infrastructure.Cache;
using App.Web.Client.Services.Abstractions;
using App.Web.Client.Services.Implementation;
using App.Web.Client.Utilities.Http;

namespace App.Web.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInternalServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IUserSessionService, UserSessionService>();
            services.AddHttpContextAccessor();

            #region Http Clients
            services.AddHttpClient();
            services.AddTransient<TodoApiAuthHandler>();
            services.AddHttpClient<ITodoApiClient, TodoApiClient>(client =>
            {
                client.BaseAddress = new Uri(config["TodoApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(config)));
            })
            .AddHttpMessageHandler<TodoApiAuthHandler>();
            #endregion

            return services;
        }
    }
}
