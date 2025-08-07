using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Web.Client.Utilities.Http;

public class TodoApiClient : ITodoApiClient
{
    private readonly HttpClient _httpClient;

    public TodoApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResult<TodolistDto>> GetTodosAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/todos?page=" + pageNumber + "&pageSize=" + pageSize, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TodolistDto>>(cancellationToken: cancellationToken);
        if (result == null)
        {
            throw new InvalidOperationException("Failed to deserialize the response to PagedResult<TodolistDto>. The response content was null or invalid.");
        }
        return result;
    }

    public async Task<TodoStatsDto> GetTodoStatsAsync(string? userId, CancellationToken cancellationToken = default)
    {
        var requestUri = string.IsNullOrEmpty(userId) ? "/todos/stats" : $"/todos/stats?userId={userId}";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoStatsDto>(cancellationToken: cancellationToken);
    }
}
