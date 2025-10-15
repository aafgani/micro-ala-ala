using Microsoft.Extensions.Hosting;

namespace App.Common.Infrastructure.Observability.Nlog;

internal static class DependencyInjection
{
    public static IHostApplicationBuilder ConfigureNlog(this IHostApplicationBuilder builder, string serviceName)
    {
        // NLog later configuration can be done here if needed

        return builder;
    }
}
