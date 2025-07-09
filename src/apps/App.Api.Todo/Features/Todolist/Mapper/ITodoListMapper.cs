using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;

namespace App.Api.Todo.Features.Todolist.Mapper;

public interface ITodoListMapper
{
    TodolistDto ToDto(ToDoList todoList);
    ToDoList ToEntity(TodolistDto todoListDto);
}
