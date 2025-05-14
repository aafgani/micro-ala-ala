using App.Api.Todo.Features.Todolist.Dtos;

namespace App.Api.Todo.Features.Todolist.Services
{
    public interface ITodolistService
    {
        Task<TodolistDto> GetAllAsync();
        Task<TodolistDto?> GetByIdAsync(int id);
        Task<TodolistDto> CreateAsync(TodolistDto dto);
        Task<bool> UpdateAsync(int id, TodolistDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
