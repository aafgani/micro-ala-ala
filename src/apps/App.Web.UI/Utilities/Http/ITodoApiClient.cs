using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Web.UI.Utilities.Http;

public interface ITodoApiClient
{
    Task<TodolistDto> CreateTodoAsync(TodolistDto todolistDto);
    Task<PagedResult<TodolistDto>> GetTodosAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

}
