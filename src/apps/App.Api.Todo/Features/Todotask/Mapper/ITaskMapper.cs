using App.Api.Todo.Features.Todotask.Dtos;
using App.Common.Domain.Dtos.Todo;
using TodoTask = App.Api.Todo.Models.TodoTask;

namespace App.Api.Todo.Features.Todotask.Mapper
{
    public interface ITaskMapper
    {
        TaskDto ToDto(TodoTask task);
        TodoTask ToEntity(TaskDto taskDto);
        TodoTask ToEntity(CreateTaskDto taskDto);
    }
}
