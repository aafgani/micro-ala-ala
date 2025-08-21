using System;
using App.Api.Todo.Features.Todos.Data;
using App.Api.Todo.Features.Todos.Mapper;
using App.Api.Todo.Features.Todos.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodosTests.ServiceTests;

public class DeleteAsyncTests
{
    [Fact]
    public async Task GivenTodoListId_DeleteAsync_ShouldReturnTrue_WhenTodoListExists()
    {
        // Arrange
        var todoListId = 1;
        var mockMapper = new Mock<ITodoMapper>();
        mockMapper.Setup(m => m.ToDto(It.IsAny<MyTodo>()))
                  .Returns(new TodolistDto { Id = todoListId });

        var mockRepository = new Mock<ITodoRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(todoListId))
                      .ReturnsAsync(new MyTodo { Id = todoListId });

        var service = new TodoService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = await service.DeleteAsync(todoListId);

        // Assert
        result.ShouldBeOfType<Result<bool, ApiError>>();
        result.Value.ShouldBeTrue();
        mockRepository.Verify(r => r.DeleteAsync(It.IsAny<MyTodo>()), Times.Once);
    }

    [Fact]
    public async Task GivenTodoListId_DeleteAsync_ShouldReturnFalse_WhenTodoListDoesNotExist()
    {
        // Arrange
        var todoListId = 1;
        var mockMapper = new Mock<ITodoMapper>();
        mockMapper.Setup(m => m.ToDto(It.IsAny<MyTodo>()))
                  .Returns(new TodolistDto { Id = todoListId });

        var mockRepository = new Mock<ITodoRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(todoListId))
                      .ReturnsAsync((MyTodo)null);

        var service = new TodoService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = await service.DeleteAsync(todoListId);

        // Assert
        result.ShouldBeOfType<Result<bool, ApiError>>();
        result.Value.ShouldBeFalse();
    }
}
