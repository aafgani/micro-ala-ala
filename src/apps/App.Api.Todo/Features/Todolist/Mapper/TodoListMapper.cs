using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Riok.Mapperly.Abstractions;

namespace App.Api.Todo.Features.Todolist.Mapper;

[Mapper]
public partial class TodoListMapper : ITodoListMapper
{
    public partial TodolistDto ToDto(ToDoList todoList);

    public partial ToDoList ToEntity(TodolistDto todoListDto);

}
