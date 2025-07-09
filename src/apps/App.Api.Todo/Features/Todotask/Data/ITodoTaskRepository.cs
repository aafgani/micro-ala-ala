using App.Common.Domain.Dtos.Todo;
using App.Common.Infrastructure.Model;
using TodoTask = App.Api.Todo.Models.TodoTask;

namespace App.Api.Todo.Features.Todotask.Data
{
    public interface ITodoTaskRepository : IRepository<TodoTask>
    {
        Task<(IEnumerable<Models.TodoTask>, int, int)> GetWithParamAsync(TodoTaskQueryParam param);
        Task<TodoTask?> GetByIdWithRelationsAsync(int id);
    }
}
