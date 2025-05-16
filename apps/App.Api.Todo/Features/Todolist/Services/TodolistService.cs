using App.Api.Todo.Features.Todolist.Dtos;

namespace App.Api.Todo.Features.Todolist.Services
{
    public class TodolistService : ITodolistService
    {
        public Task<TodolistDto> CreateAsync(TodolistDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TodolistDto> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TodolistDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, TodolistDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
