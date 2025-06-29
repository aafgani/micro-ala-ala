using App.Api.Todo.Models;
using App.Common.Infrastructure.Model;

namespace App.Api.Todo.Features.Todolist.Data
{
    public class TodolistRepository : Repository<ToDoList>, ITodolistRepository
    {
        public TodolistRepository(TodoContext db) : base(db)
        {
        }
    }
}
