using System.Net.Sockets;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace App.Web.UI.Utilities.Http.Resiliency;

public static class ResiliencePolicyFactory
{
    public static IAsyncPolicy<HttpResponseMessage> CreatePolicy(
            HttpClientResilienceOptions options,
            ILogger logger)
    {
        return Policy.WrapAsync(
            CreateCircuitBreakerPolicy(options, logger),
            Policy.WrapAsync(
                CreateRetryPolicy(options, logger),
                CreateTimeoutPolicy(options, logger)
            )
        );
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy(
        HttpClientResilienceOptions options,
        ILogger logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<SocketException>()
            .Or<TimeoutRejectedException>()
            .CircuitBreakerAsync(
                options.CircuitBreakerFailures,
                options.CircuitBreakerDelay,
                onBreak: (outcome, duration) =>
                {
                    logger.LogWarning(outcome.Exception,
                        "Circuit breaker opened for {Duration}s. Error: {Error}",
                        duration.TotalSeconds,
                        outcome.Exception?.Message ?? "Unknown error");
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("Circuit breaker half-open");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(
        HttpClientResilienceOptions options,
        ILogger logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<SocketException>()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                options.MaxRetries,
                retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                    TimeSpan.FromMilliseconds(Random.Shared.Next(0, 100)),
                onRetry: (outcome, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(outcome.Exception,
                        "Retry {RetryCount} after {Delay}s. Error: {Error}",
                        retryCount,
                        timeSpan.TotalSeconds,
                        outcome.Exception?.Message ?? "Unknown error");
                }
            );
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateTimeoutPolicy(
        HttpClientResilienceOptions options,
        ILogger logger)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(options.Timeout);
    }
}
