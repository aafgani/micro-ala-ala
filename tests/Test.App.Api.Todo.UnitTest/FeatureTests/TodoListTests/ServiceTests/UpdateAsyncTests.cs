using System;
using App.Api.Todo.Features.Todolist.Data;
using App.Api.Todo.Features.Todolist.Mapper;
using App.Api.Todo.Features.Todolist.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodoListTests.ServiceTests;

public class UpdateAsyncTests
{
    [Fact]
    public async Task UpdateAsync_ShouldUpdateTodoList_WhenTodoListExists()
    {
        // Arrange
        var mockRepository = new Mock<ITodolistRepository>();
        var mockMapper = new Mock<ITodoListMapper>();
        var service = new TodolistService(mockRepository.Object, mockMapper.Object);

        var existingTodoList = new ToDoList { Id = 1, Title = "Old Title" };
        var updateDto = new TodolistDto { Id = 1, Title = "New Title" };

        mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingTodoList);
        mockMapper.Setup(mapper => mapper.ToEntity(updateDto)).Returns(new ToDoList { Id = 1, Title = "New Title" });

        // Act
        var result = await service.UpdateAsync(1, updateDto);

        // Assert
        result.ShouldBeTrue();
        mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<ToDoList>()), Times.Once);
    }
}
