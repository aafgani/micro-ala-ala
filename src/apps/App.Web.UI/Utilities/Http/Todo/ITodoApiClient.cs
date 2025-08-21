using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Web.UI.Utilities.Http.Todo;

public interface ITodoApiClient
{
    Task<TodolistDto> CreateTodoAsync(TodolistDto todolistDto);
    Task<PagedResult<TodolistDto>> GetTodosAsync(int pageNumber = 1, int pageSize = 10, string orderBy = "createdAt", CancellationToken cancellationToken = default);

}
