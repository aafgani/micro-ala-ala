using System.Text.Json;
using Azure;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace App.Common.Infrastructure.HttpHandler;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> retryPolicy;

    public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("ApiClient initialized with base address: {BaseAddress}", _httpClient.BaseAddress);

        // Define a Polly retry policy with logging
        retryPolicy = Policy<HttpResponseMessage>
           .Handle<HttpRequestException>()
           .Or<TaskCanceledException>()
           .OrResult(r => (int)r.StatusCode >= 500 && (int)r.StatusCode < 600)
           .WaitAndRetryAsync(
           retryCount: 3,
           sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
           onRetry: (exception, delay, retryCount, context) =>
           {
               _logger.LogError("Retry {RetryCount} after {DelaySeconds}s due to: {ErrorMessage}",
               retryCount, delay.TotalSeconds, exception.Exception?.Message ?? "HTTP error");
           });
    }

    public async Task<T> SendAsync<T>(HttpRequestBuilder requestBuilder, CancellationToken cancellationToken = default)
    {
        var request = requestBuilder.Build();

        _logger.LogInformation("Sending HTTP {Method} to {Uri}", request.Method, request.RequestUri);

        var response = await retryPolicy.ExecuteAsync(() =>
        {
            var freshRequest = requestBuilder.Build();
            return _httpClient.SendAsync(freshRequest, cancellationToken);
        });
        _logger.LogInformation("Received {StatusCode} from {Uri}", response.StatusCode, response.RequestMessage?.RequestUri);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<T>(stream,
            options: null, cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Deserialization returned null");

        return result;

    }

    public async Task SendAsync(HttpRequestBuilder requestBuilder, CancellationToken cancellationToken = default)
    {
        var request = requestBuilder.Build();

        _logger.LogInformation("Sending HTTP {Method} to {Uri}", request.Method, request.RequestUri);

        var response = await retryPolicy.ExecuteAsync(() =>
{
    var freshRequest = requestBuilder.Build();
    return _httpClient.SendAsync(freshRequest, cancellationToken);
});
        response.EnsureSuccessStatusCode();

        _logger.LogInformation("Received {StatusCode} from {Uri}", response.StatusCode, response.RequestMessage?.RequestUri);
    }

}
