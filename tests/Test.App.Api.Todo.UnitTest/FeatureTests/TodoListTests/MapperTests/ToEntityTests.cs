using System;
using App.Api.Todo.Features.Todolist.Mapper;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodoListTests.MapperTests;

public class ToEntityTests
{
    [Fact]
    public void GivenTodoListDtos_ToEntity_ShouldMapCorrectly()
    {
        // Arrange.
        var dto = new TodolistDto
        {
            Id = 1,
            Title = "Test List",
            Description = "Test Description",
            CreatedAt = new DateTime(2024, 1, 1),
        };
        var mapper = new TodoListMapper();

        // Act.
        ToDoList entity = mapper.ToEntity(dto);

        // Assert.
        Assert.Equal(dto.Id, entity.Id);
        Assert.Equal(dto.Title, entity.Title);
        Assert.Equal(dto.Description, entity.Description);
        Assert.Equal(dto.CreatedAt, entity.CreatedAt);
    }

}
