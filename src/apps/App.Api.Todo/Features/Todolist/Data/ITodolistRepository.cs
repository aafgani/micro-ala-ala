using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using App.Common.Infrastructure.Model;

namespace App.Api.Todo.Features.Todolist.Data
{
    public interface ITodolistRepository : IRepository<ToDoList>
    {
        Task<(IEnumerable<ToDoList>, int, int)> GetWithParamAsync(TodoListQueryParam param);
    }
}
