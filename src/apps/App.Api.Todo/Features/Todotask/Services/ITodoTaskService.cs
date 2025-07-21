using App.Api.Todo.Features.Todotask.Dtos;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Api.Todo.Features.Todotask.Services
{
    public interface ITodoTaskService
    {
        Task<Result<PagedResult<TaskDto>, ApiError>> GetAllAsync(TodoTaskQueryParam queryParam);
        Task<Result<TaskDto?, ApiError>> GetByIdAsync(int id);
        Task<Result<TaskDto, ApiError>> CreateAsync(CreateTaskDto dto);
        Task<Result<bool, ApiError>> UpdateAsync(int id, TaskDto dto);
        Task<Result<bool, ApiError>> DeleteAsync(int id);
    }
}
