using App.Api.Todo.Features.Todos.Data;
using App.Api.Todo.Features.Todos.Mapper;
using App.Api.Todo.Features.Todos.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodosTests.ServiceTests;

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
        var mockMapper = new Mock<ITodoMapper>();
        var mockRepository = new Mock<ITodoRepository>();

        mockMapper
            .Setup(m => m.ToDto(It.IsAny<MyTodo>()))
            .Returns(new TodolistDto());

        mockRepository.
            Setup(r => r.GetWithParamAsync(queryParam))
            .ReturnsAsync((new List<MyTodo> { new MyTodo() }, 1, 1));

        var service = new TodoService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = await service.GetAllAsync(queryParam);

        // Assert
        Assert.NotNull(result);
        result.ShouldBeOfType<Result<PagedResult<TodolistDto>, ApiError>>();
        result.Value.Pagination.CurrentPage.ShouldBe(1);
        result.Value.Pagination.PageSize.ShouldBe(10);
    }
}
