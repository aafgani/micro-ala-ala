using App.Common.Domain.Dtos.TodoApi;

namespace App.Common.Domain.Dtos.TodoApi
{
    public class BaseTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsCompleted { get; set; }
        public int ToDoListId { get; set; }
        public DateTime? DueDate { get; set; }

    }
    public class TaskDto : BaseTaskDto
    {
        public int Id { get; set; }
        public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
        public ICollection<SubtaskDto> SubTasks { get; set; } = new List<SubtaskDto>();
    }

    public class SubtaskDto : BaseTaskDto
    {
        public int Id { get; set; }
    }
}
