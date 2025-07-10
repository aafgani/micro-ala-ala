using App.Api.Todo.Features.Todolist.Data;
using App.Api.Todo.Features.Todolist.Mapper;
using App.Api.Todo.Features.Todolist.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Moq;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodoListTests.ServiceTests;

public class DeleteAsyncTests
{
    [Fact]
    public async Task GivenTodoListId_DeleteAsync_ShouldReturnTrue_WhenTodoListExists()
    {
        // Arrange
        var todoListId = 1;
        var mockMapper = new Mock<ITodoListMapper>();
        mockMapper.Setup(m => m.ToDto(It.IsAny<ToDoList>()))
                  .Returns(new TodolistDto { Id = todoListId });

        var mockRepository = new Mock<ITodolistRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(todoListId))
                      .ReturnsAsync(new ToDoList { Id = todoListId });

        var service = new TodolistService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = await service.DeleteAsync(todoListId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GivenTodoListId_DeleteAsync_ShouldReturnFalse_WhenTodoListDoesNotExist()
    {
        // Arrange
        var todoListId = 1;
        var mockMapper = new Mock<ITodoListMapper>();
        mockMapper.Setup(m => m.ToDto(It.IsAny<ToDoList>()))
                  .Returns(new TodolistDto { Id = todoListId });

        var mockRepository = new Mock<ITodolistRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(todoListId))
                      .ReturnsAsync((ToDoList)null);

        var service = new TodolistService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = await service.DeleteAsync(todoListId);

        // Assert
        Assert.False(result);
    }
}
