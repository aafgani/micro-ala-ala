using App.Diagnostics.NLog;
using App.Diagnostics.OpenTelemetry;

namespace App.Diagnostics
{
    public static class DiagnosticServiceCollectionExtensions
    {
        public static IServiceCollection AddObservability(this IServiceCollection services,
        string serviceName,
        IConfiguration configuration)
        {
            var observabilityProvider = configuration.GetSection(ObservabilityOptions.SectionKey).Get<ObservabilityOptions>();

            switch (observabilityProvider?.Provider)
            {
                case ObservabilityProvider.OpenTelemetry:
                    services.ConfigureOpenTelemetry(serviceName, configuration);
                    break;
                case ObservabilityProvider.Nlog:
                    services.ConfigureNLog(serviceName, configuration);
                    break;
                default:
                    throw new NotSupportedException($"Observability provider '{observabilityProvider?.Provider}' is not supported.");
            }
            return services;
        }
    }
}
