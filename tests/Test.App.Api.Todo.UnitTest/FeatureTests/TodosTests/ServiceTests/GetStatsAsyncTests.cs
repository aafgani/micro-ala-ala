using System;
using App.Api.Todo.Features.Todos.Data;
using App.Api.Todo.Features.Todos.Mapper;
using App.Api.Todo.Features.Todos.Services;
using App.Common.Domain.Dtos.Todo;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodosTests.ServiceTests;

public class GetStatsAsyncTests
{
    [Fact]
    public async Task GetStatsAsync_ShouldReturnTodoStats_WhenUserIdIsValid()
    {
        // Arrange
        var userId = "1";
        var repo = new Mock<ITodoRepository>();
        var expectedStats = new TodoStatsDto
        {
            TotalTodos = 5,
            CompletedTodos = 3,
            PendingTodos = 2,
            TodaysCreated = 1,
            TodaysCompleted = 0,
            CompletionRate = 60.0
        };
        repo.Setup(s => s.GetStatsAsync(userId)).ReturnsAsync(expectedStats);
        var service = new TodoService(repo.Object, Mock.Of<ITodoMapper>());

        // Act
        var result = await service.GetStatsAsync(userId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TotalTodos.ShouldBe(expectedStats.TotalTodos);
        result.Value.CompletedTodos.ShouldBe(expectedStats.CompletedTodos);
        result.Value.PendingTodos.ShouldBe(expectedStats.PendingTodos);
        result.Value.TodaysCreated.ShouldBe(expectedStats.TodaysCreated);
        result.Value.TodaysCompleted.ShouldBe(expectedStats.TodaysCompleted);
        result.Value.CompletionRate.ShouldBe(expectedStats.CompletionRate);
    }

}
