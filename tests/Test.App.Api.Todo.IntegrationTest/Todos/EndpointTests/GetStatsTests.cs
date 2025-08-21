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
        // Start transaction
        await using var transaction = await TodoContext.Database.BeginTransactionAsync();

        try
        {
            // Arrange
            var userId = "1";
            var todos = new List<MyTodo>
            {
                new MyTodo { Title = "Todo 1", IsCompleted = true, CreatedBy = userId, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new MyTodo { Title = "Todo 2", IsCompleted = false, CreatedBy = userId, CreatedAt = DateTime.UtcNow },
                new MyTodo { Title = "Todo 3", IsCompleted = true, CreatedBy = "2", CreatedAt = DateTime.UtcNow }
            };

            TodoContext.MyTodo.AddRange(todos);
            await TodoContext.SaveChangesAsync();
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
            stats.TodaysCreated.ShouldBe(2);
            stats.TodaysCompleted.ShouldBe(1);
        }
        finally
        {
            // Rollback transaction to clean up
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task GivenValidRequestWithUserId_GetTodoStats_ShouldReturnOkAsync()
    {
        // Arrange
        var userId = "1"; // Assuming a user with ID 1 exists
        var todos = new List<MyTodo>
        {
            new MyTodo { Title = "Todo 1", IsCompleted = true, CreatedBy = userId, CreatedAt = DateTime.UtcNow.AddDays(-1) }, // Yesterday
            new MyTodo { Title = "Todo 2", IsCompleted = false, CreatedBy = userId, CreatedAt = DateTime.UtcNow }, // Today
            new MyTodo { Title = "Todo 3", IsCompleted = true, CreatedBy = "2", CreatedAt = DateTime.UtcNow } // Today
        };
        TodoContext.MyTodo.AddRange(todos);
        TodoContext.SaveChanges();
        AuthenticateAsUser(userId);

        // Act
        var response = await Client.GetAsync($"/todos/stats?userId={userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var stats = await response.Content.ReadFromJsonAsync<TodoStatsDto>();
        stats.ShouldNotBeNull();
        stats.TotalTodos.ShouldBe(2); // Assuming there are todos for userId 1
        stats.CompletedTodos.ShouldBe(1); // Only Todo 1 is completed
        stats.PendingTodos.ShouldBe(1); // Only Todo 2 is pending
        stats.TodaysCreated.ShouldBe(1); // Only Todo 2 is created today
        stats.TodaysCompleted.ShouldBe(0); // No todos completed today for userId
    }
}
