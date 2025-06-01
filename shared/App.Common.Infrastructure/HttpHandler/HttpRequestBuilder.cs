using System.Text;
using System.Text.Json;

namespace App.Common.Infrastructure.HttpHandler;

public class HttpRequestBuilder
{
    private readonly HttpRequestMessage _request = new();

    public HttpRequestBuilder WithMethod(HttpMethod method)
    {
        _request.Method = method;
        return this;
    }

    public HttpRequestBuilder WithUri(Uri uri)
    {
        _request.RequestUri = uri;
        return this;
    }

    public HttpRequestBuilder WithHeaders(IDictionary<string, string> headers)
    {
        foreach (var header in headers)
        {
            _request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return this;
    }

    public HttpRequestBuilder WithHeader(string name, string value)
    {
        _request.Headers.TryAddWithoutValidation(name, value);
        return this;
    }

    public HttpRequestBuilder WithJsonBody(object body)
    {
        var json = JsonSerializer.Serialize(body);
        _request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return this;
    }

    public HttpRequestMessage Build() => _request;
}
