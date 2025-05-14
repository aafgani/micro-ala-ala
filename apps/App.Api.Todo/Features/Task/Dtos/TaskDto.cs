using App.Api.Todo.Features.Tags.Dtos;
using System.ComponentModel.DataAnnotations;

namespace App.Api.Todo.Features.Task.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Notes { get; set; }

        public bool IsCompleted { get; set; }
        public ICollection<TagDto> Tags { get; set; }
    }
}
