using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Api.Todo.Features.Todos.Services;

public interface ITodoService
{
    Task<Result<PagedResult<TodolistDto>, ApiError>> GetAllAsync(TodoListQueryParam queryParam);
    Task<Result<TodolistDto?, ApiError>> GetByIdAsync(int id);
    Task<Result<TodolistDto, ApiError>> CreateAsync(TodolistDto dto);
    Task<Result<bool, ApiError>> UpdateAsync(int id, TodolistDto dto);
    Task<Result<bool, ApiError>> DeleteAsync(int id);
    Task<Result<TodoStatsDto, ApiError>> GetStatsAsync(string? userId = null);
}
