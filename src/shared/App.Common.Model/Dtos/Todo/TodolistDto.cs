namespace App.Common.Domain.Dtos.Todo
{
    public class TodolistDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DueDate { get; set; }
        public string UserId { get; set; }
    }
}
