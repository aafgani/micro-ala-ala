using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace App.Common.Infrastructure.Observability.OpenTelemetry;

internal static class DependencyInjection
{
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder, string serviceName)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        string otelEndpoint = builder.Configuration.GetSection(ObservabilityOptions.SectionKey).Get<ObservabilityOptions>()?.Endpoint ?? "http://localhost:4317";

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceVersion: "1.0.0")
            .AddAttributes(new[]
            {
                new KeyValuePair<string, object>("deployment.environment", environment),
                new KeyValuePair<string, object>("host.name", Environment.MachineName)
            });

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.SetResourceBuilder(resourceBuilder);
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;
            logging.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otelEndpoint + "/v1/logs");
                options.Protocol = OtlpExportProtocol.HttpProtobuf;
            });
        });

        // builder.Services.AddOpenTelemetry()
        //      .WithTracing(tracing =>
        //      {
        //          tracing
        //              .SetResourceBuilder(resourceBuilder)
        //              .AddSource(serviceName)
        //              .AddAspNetCoreInstrumentation()
        //              .AddHttpClientInstrumentation()
        //              .AddNpgsql()
        //              .AddSqlClientInstrumentation(opt => opt.SetDbStatementForText = true)
        //              //.AddRedisInstrumentation(opt => opt.SetVerboseDatabaseStatements = true)
        //              .AddOtlpExporter(options =>
        //              {
        //                  options.Endpoint = new Uri("http://172.19.14.206:4318/v1/traces");
        //                  options.Protocol = OtlpExportProtocol.HttpProtobuf;
        //              });
        //      })
        //      .WithMetrics(metrics =>
        //      {
        //          metrics
        //              .SetResourceBuilder(resourceBuilder)
        //              .AddAspNetCoreInstrumentation()
        //              .AddHttpClientInstrumentation()
        //              .AddRuntimeInstrumentation()
        //              .AddProcessInstrumentation()
        //              .AddMeter("Microsoft.AspNetCore.Hosting")
        //              .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
        //              .AddMeter("System.Net.Http")
        //              .AddMeter("System.Net.NameResolution")
        //              .AddOtlpExporter(options =>
        //              {
        //                  options.Endpoint = new Uri("http://172.19.14.206:4318/v1/metrics");
        //                  options.Protocol = OtlpExportProtocol.HttpProtobuf;
        //              });
        //      });

        return builder;
    }
}
