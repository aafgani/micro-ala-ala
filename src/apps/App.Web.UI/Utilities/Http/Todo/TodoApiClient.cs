using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using App.Web.UI.Models.Dto;

namespace App.Web.UI.Utilities.Http.Todo;

public class TodoApiClient : BaseApiClient, ITodoApiClient
{
    public TodoApiClient(HttpClient httpClient, ILogger<TodoApiClient> logger) : base(httpClient, logger)
    {
    }

    public async Task<Result<TodolistDto, ApiError>> CreateTodoAsync(TodolistDto todolistDto)
    {
        var result = new TodolistDto();
        try
        {
            return await SendAsync<TodolistDto, TodolistDto>(HttpMethod.Post, "/todos", todolistDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating todo");
            return new ApiError(ex.Message);
        }
    }

    public async Task<Result<PagedResult<TodolistDto>, ApiError>> GetTodosAsync(int pageNumber = 1, int pageSize = 10, string orderBy = "createdAt", string sortDirection = "desc", CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $"page={pageNumber}&pageSize={pageSize}&orderBy={orderBy}&SortDirection={sortDirection}";

            return await GetAsync<TodolistDto>("/todos", query, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching todos");
            return new ApiError(ex.Message);
        }
    }

    public async Task<Result<bool, ApiError>> UpdateTodoAsync(int id, TodoDto todo, CancellationToken cancellationToken = default)
    {
        try
        {
            return await SendAsync<TodoDto, bool>(HttpMethod.Put, $"/todos/{id}", todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating todoId : {id}");
            return new ApiError(ex.Message);
        }
    }
}
