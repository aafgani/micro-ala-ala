using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using App.Common.Infrastructure.Model;

namespace App.Api.Todo.Features.Todos.Data;

public interface ITodoRepository : IRepository<MyTodo>
{
    Task<(IEnumerable<MyTodo>, int, int)> GetWithParamAsync(TodoListQueryParam param);
}
