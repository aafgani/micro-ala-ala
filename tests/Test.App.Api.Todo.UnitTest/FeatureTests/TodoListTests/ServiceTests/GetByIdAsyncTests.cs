
using App.Api.Todo.Features.Todolist.Data;
using App.Api.Todo.Features.Todolist.Mapper;
using App.Api.Todo.Features.Todolist.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Moq;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodoListTests.ServiceTests;

public class GetByIdAsyncTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoList_WhenExists()
    {
        // Arrange
        var todoListId = 1;
        var todoListEntity = new ToDoList { Id = todoListId, Title = "Test List" };
        var todoListDto = new TodolistDto { Id = todoListId, Title = "Test List" };

        var mockRepository = new Mock<ITodolistRepository>();
        var mockMapper = new Mock<ITodoListMapper>();

        mockRepository.Setup(repo => repo.GetByIdAsync(todoListId)).ReturnsAsync(todoListEntity);
        mockMapper.Setup(mapper => mapper.ToDto(todoListEntity)).Returns(todoListDto);

        var service = new TodolistService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = await service.GetByIdAsync(todoListId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(todoListId, result.Id);
        Assert.Equal("Test List", result.Title);
    }
}
