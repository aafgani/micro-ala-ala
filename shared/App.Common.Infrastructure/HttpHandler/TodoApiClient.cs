using Microsoft.Extensions.Logging;

namespace App.Common.Infrastructure.HttpHandler;

public class TodoApiClient : ApiClient, IApiClient
{
    public TodoApiClient(HttpClient httpClient, ILogger<TodoApiClient> logger) : base(httpClient, logger)
    {
    }
}
