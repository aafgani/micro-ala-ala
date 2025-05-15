using App.Api.Todo.Features.Todotask.Dtos;

namespace App.Api.Todo.Features.Todolist.Dtos
{
    public class TodolistDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public ICollection<TaskDto> Tasks { get; set; }
    }
}
