using App.Api.Todo.Features.Todotask.Data;
using App.Api.Todo.Features.Todotask.Dtos;
using App.Api.Todo.Features.Todotask.Mapper;
using App.Api.Todo.Features.Todotask.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodoTaskTests.ServiceTests;

public class GetAllAsyncTests
{
    [Fact]
    public async Task GivenTodoTaskQueryParam_GetAllAsync_ShouldReturnPagedResult_WhenQueryParamIsValid()
    {
        // Arrange
        var taskMapper = new Mock<ITaskMapper>();
        var todoTaskRepository = new Mock<ITodoTaskRepository>();
        var todoTaskService = new TodoTaskService(todoTaskRepository.Object, taskMapper.Object);
        var queryParam = new TodoTaskQueryParam
        {
            Page = 1,
            PageSize = 10,
            SortBy = "CreatedDate",
            SortDirection = "asc"
        };
        taskMapper.Setup(m => m.ToDto(It.IsAny<TodoTask>())).Returns(new TaskDto
        {
            Id = 1,
            Title = "Test Task",
            DueDate = DateTime.UtcNow
        });
        todoTaskRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<TodoTask>
            {
                new TodoTask { Id = 1, Title = "Test Task 1", DueDate = DateTime.UtcNow },
                new TodoTask { Id = 2, Title = "Test Task 2", DueDate = DateTime.UtcNow }
            });

        // Act
        var result = await todoTaskService.GetAllAsync(queryParam);

        // Assert
        result.ShouldBeOfType<Result<PagedResult<TaskDto>, ApiError>>();
        result.Value.Pagination.CurrentPage.ShouldBe(1);
        result.Value.Pagination.PageSize.ShouldBe(10);
    }
}
