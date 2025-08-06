using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;

namespace App.Api.Todo.Features.Todos.Mapper;

public interface ITodoMapper
{
    TodolistDto ToDto(MyTodo todo);
    MyTodo ToEntity(TodolistDto todoDto);
}
