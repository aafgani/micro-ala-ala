using App.Api.Todo.Features.Tags.Dtos;

namespace App.Api.Todo.Features.Todotask.Dtos
{
    public class CreateTaskDto : BaseTaskDto
    {
        public ICollection<CreateSubtaskDto> SubTasks { get; set; }
    }
    public class CreateSubtaskDto : BaseTaskDto
    {
    }
}
