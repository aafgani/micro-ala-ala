using App.Api.Todo.Features.Todotask.Dtos;
using App.Common.Abstractions.Models;
using Task = App.Api.Todo.Models.Task;

namespace App.Api.Todo.Features.Todotask.Data
{
    public interface ITodoTaskRepository : IRepository<Task>
    {
        Task<(IEnumerable<Models.Task>, int, int)> GetWithParamAsync(TodoTaskQueryParam param);
    }
}
