using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace App.Diagnostics.OpenTelemetry;

internal static class ConfigureOpenTelemetryServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOpenTelemetry(this IServiceCollection services,
           string serviceName,
           IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        string otelEndpoint = configuration.GetSection(ObservabilityOptions.SectionKey).Get<ObservabilityOptions>()?.Endpoint ?? "http://localhost:4317";

        // create the resource that references the service name passed in
        var resource = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: "1.0")
             .AddAttributes(new[]
            {
                new KeyValuePair<string, object>("deployment.environment", environment),
                new KeyValuePair<string, object>("host.name", Environment.MachineName)
            });

        // add the OpenTelemetry services
        var otelBuilder = services.AddOpenTelemetry();

        otelBuilder
        // add the logging providers
            .WithLogging(options =>
            {
                options.SetResourceBuilder(resource);
                // options.IncludeFormattedMessage = true;
                // options.ParseStateValues = true;
                // options.AddConsoleExporter();
                options.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(otelEndpoint + "/v1/logs");
                    otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
            })
        // add the metrics providers
        .WithMetrics(metrics =>
        {
            metrics
              .SetResourceBuilder(resource)
              .AddRuntimeInstrumentation()
              .AddAspNetCoreInstrumentation()
              .AddHttpClientInstrumentation()
              .AddProcessInstrumentation()
              .AddEventCountersInstrumentation(c =>
              {
                  c.AddEventSources(
                        "Microsoft.AspNetCore.Hosting",
                        "Microsoft-AspNetCore-Server-Kestrel",
                        "System.Net.Http",
                        "System.Net.Sockets");
              })
              .AddMeter(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Server.Kestrel",
                "System.Net.Http",
                "System.Net.NameResolution"
                )
              .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(otelEndpoint + "/v1/metrics");
                    otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
        })
        // add the tracing providers
        // .WithTracing(tracing =>
        // {
        //     tracing.SetResourceBuilder(resource)
        //                 .AddAspNetCoreInstrumentation()
        //                 .AddHttpClientInstrumentation()
        //                 .AddSqlClientInstrumentation();
        // })
        ;

        return services;
    }
}
