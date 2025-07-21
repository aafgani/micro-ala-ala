using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Api.Todo.Features.Todolist.Services
{
    public interface ITodolistService
    {
        Task<Result<PagedResult<TodolistDto>, ApiError>> GetAllAsync(TodoListQueryParam queryParam);
        Task<Result<TodolistDto?, ApiError>> GetByIdAsync(int id);
        Task<Result<TodolistDto, ApiError>> CreateAsync(TodolistDto dto);
        Task<Result<bool, ApiError>> UpdateAsync(int id, TodolistDto dto);
        Task<Result<bool, ApiError>> DeleteAsync(int id);
    }
}
