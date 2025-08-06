using System.Net.Http.Json;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.Todos.EndpointTests;

public class GetStatsTests : BaseIntegrationTest
{
    public GetStatsTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenValidRequest_GetTodoStats_ShouldReturnOkAsync()
    {
        // Arrange
        var userId = "1"; //
        var todos = new List<MyTodo>
        {
            new MyTodo { Title = "Todo 1", IsCompleted = true, CreatedBy = userId, CreatedAt = DateTime.UtcNow.AddDays(-1) }, // Yesterday
            new MyTodo { Title = "Todo 2", IsCompleted = false, CreatedBy = userId, CreatedAt = DateTime.UtcNow }, // Today
            new MyTodo { Title = "Todo 3", IsCompleted = true, CreatedBy = userId, CreatedAt = DateTime.UtcNow } // Today
        };
        TodoContext.MyTodo.AddRange(todos);
        TodoContext.SaveChanges();
        AuthenticateAsUser(userId);

        // Act
        var response = await Client.GetAsync("/todos/stats");

        // Assert
        response.EnsureSuccessStatusCode();
        var stats = await response.Content.ReadFromJsonAsync<TodoStatsDto>();
        stats.ShouldNotBeNull();
        stats.TotalTodos.ShouldBe(3);
        stats.CompletedTodos.ShouldBe(2);
        stats.PendingTodos.ShouldBe(1);
        stats.TodaysCreated.ShouldBe(2); // Todo 2 and Todo 3 are both created today
        stats.TodaysCompleted.ShouldBe(1); // Only Todo 3 is completed today
        // stats.YesterdaysCreated.ShouldBe(1); // Only Todo 1 is created yesterday
        // stats.YesterdaysCompleted.ShouldBe(1); // Todo 1 is completed yesterday
    }
}
