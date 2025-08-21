using System.Net.Http.Headers;
using App.Common.Infrastructure.Cache;
using App.Web.UI.Utilities.Http;
using App.Web.UI.Utilities.Http.Todo;
using App.Web.UI.Utilities.Session;

namespace App.Web.UI.Extensions;

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
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddHttpMessageHandler<TodoApiAuthHandler>();
        #endregion

        return services;
    }

}
