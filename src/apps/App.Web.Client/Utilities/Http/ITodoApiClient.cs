using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Web.Client.Utilities.Http;

public interface ITodoApiClient
{
    Task<PagedResult<TodolistDto>> GetTodosAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<TodoStatsDto> GetTodoStatsAsync(string userId = null, CancellationToken cancellationToken = default);
}
