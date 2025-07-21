using App.Api.Todo.Features.Todolist.Data;
using App.Api.Todo.Features.Todolist.Mapper;
using App.Api.Todo.Features.Todolist.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodoListTests.ServiceTests;

public class GetAllAsyncTests
{
    [Fact]
    public async Task GivenTodoListQueryParam_GetAllAsync_ShouldReturnPagedResult_WhenQueryParamIsValid()
    {
        // Arrange
        var queryParam = new TodoListQueryParam
        {
            Page = 1,
            PageSize = 10,
            SortBy = "CreatedDate",
            SortDirection = "asc"
        };
        var mockMapper = new Mock<ITodoListMapper>();
        var mockRepository = new Mock<ITodolistRepository>();

        mockMapper
            .Setup(m => m.ToDto(It.IsAny<ToDoList>()))
            .Returns(new TodolistDto());

        mockRepository.
            Setup(r => r.GetWithParamAsync(queryParam))
            .ReturnsAsync((new List<ToDoList> { new ToDoList() }, 1, 1));

        var service = new TodolistService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = await service.GetAllAsync(queryParam);

        // Assert
        Assert.NotNull(result);
        result.ShouldBeOfType<Result<PagedResult<TodolistDto>, ApiError>>();
        result.Value.Pagination.CurrentPage.ShouldBe(1);
        result.Value.Pagination.PageSize.ShouldBe(10);
    }
}
