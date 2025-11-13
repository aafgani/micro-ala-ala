using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using App.Web.UI.Models.Dto;

namespace App.Web.UI.Utilities.Http.Todo;

public interface ITodoApiClient
{
    Task<Result<TodolistDto, ApiError>> CreateTodoAsync(TodolistDto todolistDto);
    Task<Result<PagedResult<TodolistDto>, ApiError>> GetTodosAsync(int pageNumber = 1, int pageSize = 10, string orderBy = "createdAt", string sortDirection = "desc", CancellationToken cancellationToken = default);

    Task<Result<bool, ApiError>> UpdateTodoAsync(int id, TodoDto todo, CancellationToken cancellationToken = default);

}
