using App.Common.Infrastructure.Observability.Nlog;
using App.Common.Infrastructure.Observability.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace App.Common.Infrastructure.Observability;

public static class DependencyInjection
{
    public static IHostApplicationBuilder ConfigureObservability(this IHostApplicationBuilder builder, string serviceName)
    {
        var observabilityOptions = builder.Configuration.GetSection(ObservabilityOptions.SectionKey).Get<ObservabilityOptions>();

        switch (observabilityOptions?.Provider)
        {
            case ObservabilityProvider.OpenTelemetry:
                builder.ConfigureOpenTelemetry(serviceName);
                break;
            case ObservabilityProvider.Nlog:
                builder.ConfigureNlog(serviceName);
                break;
            default:
                throw new NotSupportedException($"Observability provider '{observabilityOptions?.Provider}' is not supported.");
        }
        return builder;
    }
}
