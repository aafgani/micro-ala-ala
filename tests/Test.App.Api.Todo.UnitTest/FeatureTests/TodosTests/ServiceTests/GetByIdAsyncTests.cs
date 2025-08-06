using App.Api.Todo.Features.Todos.Data;
using App.Api.Todo.Features.Todos.Mapper;
using App.Api.Todo.Features.Todos.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodosTests.ServiceTests;

public class GetByIdAsyncTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoList_WhenExists()
    {
        // Arrange
        var todoListId = 1;
        var todoListEntity = new MyTodo { Id = todoListId, Title = "Test List" };
        var todoListDto = new TodolistDto { Id = todoListId, Title = "Test List" };

        var mockRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<ITodoMapper>();

        mockRepository.Setup(repo => repo.GetByIdAsync(todoListId)).ReturnsAsync(todoListEntity);
        mockMapper.Setup(mapper => mapper.ToDto(todoListEntity)).Returns(todoListDto);

        var service = new TodoService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = await service.GetByIdAsync(todoListId);

        // Assert
        result.ShouldBeOfType<Result<TodolistDto, ApiError>>();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(todoListId);
        result.Value.Title.ShouldBe("Test List");
    }
}
