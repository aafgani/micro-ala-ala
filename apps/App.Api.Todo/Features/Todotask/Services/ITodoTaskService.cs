using App.Api.Todo.Features.Todotask.Dtos;
using App.Common.Domain.Pagination;

namespace App.Api.Todo.Features.Todotask.Services
{
    public interface ITodoTaskService
    {
        Task<PagedResult<TaskDto>> GetAllAsync(TodoTaskQueryParam queryParam);
        Task<TaskDto?> GetByIdAsync(int id);
        Task<TaskDto> CreateAsync(CreateTaskDto dto);
        Task<bool> UpdateAsync(int id, TaskDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
