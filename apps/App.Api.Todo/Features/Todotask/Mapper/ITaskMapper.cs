using App.Api.Todo.Features.Todotask.Dtos;
using Task = App.Api.Todo.Models.Task;

namespace App.Api.Todo.Features.Todotask.Mapper
{
    public interface ITaskMapper
    {
        TaskDto ToDto(Task task);
        Task ToEntity(TaskDto taskDto);
        Task ToEntity(CreateTaskDto taskDto);
    }
}
