using App.Api.Todo.Features.Todotask.Dtos;

namespace App.Common.Domain.Dtos.Todo
{
    public class CreateTaskDto : BaseTaskDto
    {
        public ICollection<CreateSubtaskDto> SubTasks { get; set; }
    }

    public class UpdateTaskDto : TaskDto
    {
    }

    public class CreateSubtaskDto : BaseTaskDto
    {
    }
}
