using System.Net.Http.Headers;
using App.Common.Infrastructure.Cache;
using App.Web.UI.Utilities.Http.Resiliency;
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
        services.AddHttpClients(config);
        return services;
    }

    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<TodoApiAuthHandler>();
        services.AddHttpClient<ITodoApiClient, TodoApiClient>((serviceProvider, client) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TodoApiClient>>();
            logger.LogInformation("Configuring TodoApiClient with base URL: {BaseUrl}", config["TodoApi:BaseUrl"]);
            client.BaseAddress = new Uri(config["TodoApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(config)));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddHttpMessageHandler<TodoApiAuthHandler>()
        .AddPolicyHandler((serviceProvider, _) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TodoApiClient>>();
            logger.LogInformation("Creating resilience policies for TodoApiClient");
            return ResiliencePolicyFactory.CreatePolicy(
                new HttpClientResilienceOptions(),
                logger);
        })
        ;

        return services;
    }
}
