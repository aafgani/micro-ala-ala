using App.Common.Domain.Dtos.Todo;

namespace App.Api.Todo.Features.Todotask.Dtos
{
    public class BaseTaskDto
    {
        public string Title { get; set; }
        public string Notes { get; set; }
        public bool IsCompleted { get; set; }
        public int ToDoListId { get; set; }
        public DateTime? DueDate { get; set; }
        public string AssignedUserName { get; set; }

    }
    public class TaskDto : BaseTaskDto
    {
        public int Id { get; set; }
        public ICollection<TagDto> Tags { get; set; }
        public ICollection<SubtaskDto> SubTasks { get; set; }
    }

    public class SubtaskDto : BaseTaskDto
    {
        public int Id { get; set; }
    }
}
