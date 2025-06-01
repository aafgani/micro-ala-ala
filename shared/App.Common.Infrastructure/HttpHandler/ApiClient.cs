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
           .Handle<RequestFailedException>(ex => IsTransientError(ex))
           .WaitAndRetryAsync(
           retryCount: 3,
           sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
           onRetry: (exception, delay, retryCount, context) =>
           {
               _logger.LogError($"Retry {retryCount} after {delay.TotalSeconds}s due to: {exception.Exception.Message}");
           });
    }

    public async Task<T> SendAsync<T>(HttpRequestBuilder requestBuilder, CancellationToken cancellationToken = default)
    {
        var request = requestBuilder.Build();

        _logger.LogInformation("Sending HTTP {Method} to {Uri}", request.Method, request.RequestUri);

        var response = await retryPolicy.ExecuteAsync(() => _httpClient.SendAsync(request, cancellationToken));
        _logger.LogInformation("Received {StatusCode} from {Uri}", response.StatusCode, response.RequestMessage?.RequestUri);

        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Deserialization returned null");

    }

    public async Task SendAsync(HttpRequestBuilder requestBuilder, CancellationToken cancellationToken = default)
    {
        var request = requestBuilder.Build();

        _logger.LogInformation("Sending HTTP {Method} to {Uri}", request.Method, request.RequestUri);

        var response = await retryPolicy.ExecuteAsync(() => _httpClient.SendAsync(request, cancellationToken));
        response.EnsureSuccessStatusCode();

        _logger.LogInformation("Received {StatusCode} from {Uri}", response.StatusCode, response.RequestMessage?.RequestUri);
    }
    private bool IsTransientError(RequestFailedException ex)
    {
        return (ex.Status >= 500 && ex.Status < 600);
    }
}
