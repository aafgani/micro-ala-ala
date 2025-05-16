using App.Api.Todo.Features.Todotask.Dtos;

namespace App.Api.Todo.Features.Todotask.Mapper
{
    public interface ITaskMapper
    {
        TaskDto ToDto(Task task);
        Task ToEntity(TaskDto taskDto);
    }
}
