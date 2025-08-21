using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Web.UI.Utilities.Http.Todo;

public class TodoApiClient : BaseApiClient, ITodoApiClient
{
    public TodoApiClient(HttpClient httpClient, ILogger<TodoApiClient> logger) : base(httpClient, logger)
    {
    }

    public async Task<TodolistDto> CreateTodoAsync(TodolistDto todolistDto)
    {
        return await PostAsync<TodolistDto, TodolistDto>("/todos", todolistDto);
    }

    public async Task<PagedResult<TodolistDto>> GetTodosAsync(int pageNumber = 1, int pageSize = 10, string orderBy = "createdAt", CancellationToken cancellationToken = default)
    {
        var query = $"page={pageNumber}&pageSize={pageSize}&orderBy={orderBy}";

        return await GetAsync<TodolistDto>("/todos", query, cancellationToken);
    }
}
