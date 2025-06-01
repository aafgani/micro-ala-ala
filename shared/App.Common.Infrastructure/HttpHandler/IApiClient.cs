using System;

namespace App.Common.Infrastructure.HttpHandler;

public interface IApiClient
{
    Task<T> SendAsync<T>(HttpRequestBuilder requestBuilder, CancellationToken cancellationToken = default);
    Task SendAsync(HttpRequestBuilder requestBuilder, CancellationToken cancellationToken = default);
}
