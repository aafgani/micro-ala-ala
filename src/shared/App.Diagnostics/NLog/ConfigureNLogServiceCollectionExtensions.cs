using System;

namespace App.Diagnostics.NLog;

internal static class ConfigureNLogServiceCollectionExtensions
{
    public static IServiceCollection ConfigureNLog(this IServiceCollection services,
               string serviceName,
               IConfiguration configuration)
    {
        return services;
    }
}
