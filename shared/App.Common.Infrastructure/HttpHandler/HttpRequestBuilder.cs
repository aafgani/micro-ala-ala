using System.Text;
using System.Text.Json;

namespace App.Common.Infrastructure.HttpHandler;

public class HttpRequestBuilder
{
    private HttpMethod? _method;
    private Uri? _requestUri;
    private readonly Dictionary<string, string> _headers = new();
    private HttpContent? _content;

    public HttpRequestBuilder WithMethod(HttpMethod method)
    {
        _method = method;
        return this;
    }

    public HttpRequestBuilder WithUri(Uri uri)
    {
        _requestUri = uri;
        return this;
    }

    public HttpRequestBuilder WithHeaders(IDictionary<string, string> headers)
    {
        foreach (var header in headers)
        {
            _headers[header.Key] = header.Value;
        }
        return this;
    }

    public HttpRequestBuilder WithHeader(string name, string value)
    {
        _headers[name] = value;
        return this;
    }

    public HttpRequestBuilder WithJsonBody(object body)
    {
        var json = JsonSerializer.Serialize(body);
        _content = new StringContent(json, Encoding.UTF8, "application/json");
        return this;
    }

    public HttpRequestMessage Build()
    {
        var request = new HttpRequestMessage();
        if (_method != null) request.Method = _method;
        if (_requestUri != null) request.RequestUri = _requestUri;
        if (_content != null) request.Content = _content;

        foreach (var header in _headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return request;
    }
}
