using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Riok.Mapperly.Abstractions;

namespace App.Api.Todo.Features.Todos.Mapper;

[Mapper]
public partial class TodoMapper : ITodoMapper
{
    [MapProperty(nameof(MyTodo.AssignTo), nameof(TodolistDto.UserId))]
    [MapProperty(nameof(MyTodo.Notes), nameof(TodolistDto.Description))]
    public partial TodolistDto ToDto(MyTodo todo);

    [MapProperty(nameof(TodolistDto.UserId), nameof(MyTodo.AssignTo))]
    [MapProperty(nameof(TodolistDto.Description), nameof(MyTodo.Notes))]
    public partial MyTodo ToEntity(TodolistDto todoDto);
}
