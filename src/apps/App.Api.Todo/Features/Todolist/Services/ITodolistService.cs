using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Api.Todo.Features.Todolist.Services
{
    public interface ITodolistService
    {
        Task<PagedResult<TodolistDto>> GetAllAsync(TodoListQueryParam queryParam);
        Task<TodolistDto?> GetByIdAsync(int id);
        Task<TodolistDto> CreateAsync(TodolistDto dto);
        Task<bool> UpdateAsync(int id, TodolistDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
