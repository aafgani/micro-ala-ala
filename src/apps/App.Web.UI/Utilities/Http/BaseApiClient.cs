using System.Text.Json;
using App.Common.Domain.Pagination;

namespace App.Web.UI.Utilities.Http;

public class BaseApiClient
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;
    protected readonly JsonSerializerOptions _jsonOptions;

    public BaseApiClient(HttpClient httpClient, ILogger logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task LogRequestAsync<T>(string endpoint, T payload, string method = "POST")
    {
        var jsonPayload = JsonSerializer.Serialize(payload, _jsonOptions);
        _logger.LogInformation(
            "Sending {Method} request to {Endpoint}. Payload: {Payload}",
            method,
            endpoint,
            jsonPayload
        );
    }

    private void LogResponse(string endpoint, HttpResponseMessage response)
    {
        _logger.LogInformation(
            "Received response from {Endpoint}. Status: {StatusCode}",
            endpoint,
            response.StatusCode
        );
    }

    private async Task LogErrorAsync<T>(Exception ex, string endpoint, T payload)
    {
        _logger.LogError(
            ex,
            "Failed request to {Endpoint}. Payload was: {Payload}",
            endpoint,
            JsonSerializer.Serialize(payload, _jsonOptions)
        );
    }

    protected async Task<TResponse> SendAsync<TRequest, TResponse>(
    HttpMethod httpMethod,
    string endpoint,
    TRequest payload,
    CancellationToken cancellationToken = default)
    {
        await LogRequestAsync(endpoint, payload);

        var response = await _httpClient.SendAsync(new HttpRequestMessage(httpMethod, endpoint)
        {
            Content = JsonContent.Create(payload)
        });
        LogResponse(endpoint, response);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed request to {Endpoint}. Status: {StatusCode}, Error: {Error}",
                endpoint, response.StatusCode, error);

            throw new HttpRequestException(
                $"Failed request to {endpoint}. Status: {response.StatusCode}, Error: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);

        if (result == null)
        {
            throw new InvalidOperationException($"Response from {endpoint} was null");
        }

        return result;
    }


    protected async Task<PagedResult<T>> GetAsync<T>(string endpoint, string query, CancellationToken cancellationToken = default)
    {
        var url = $"{endpoint}?{query}";
        _logger.LogInformation("Getting paged data from {Url}", url);

        var response = await _httpClient.GetAsync(url, cancellationToken);
        LogResponse(endpoint, response);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PagedResult<T>>(cancellationToken: cancellationToken);

        if (result == null)
        {
            throw new InvalidOperationException($"Failed to deserialize the response from {endpoint} to PagedResult<{typeof(T).Name}>.");
        }

        return result;
    }
}
