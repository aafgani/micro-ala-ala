using App.Api.Todo.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.Todos.RepoTests;

public class GetStatsAsyncTests : BaseIntegrationTest, IAsyncLifetime
{
    private IDbContextTransaction _transaction;
    public GetStatsAsyncTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    public async Task InitializeAsync()
    {
        _transaction = await TodoContext.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
    }

    [Fact]
    public async Task GivenValidUserId_GetStatsAsync_ShouldReturnTodoStats()
    {
        // Arrange
        var userId = "1";
        var today = DateTime.UtcNow.Date;
        var todos = new List<MyTodo>
        {
            new MyTodo { Title = "Todo 1", IsCompleted = true, CreatedBy = userId, CreatedAt = today.AddDays(-1) },
            new MyTodo { Title = "Todo 2", IsCompleted = false, CreatedBy = userId, CreatedAt = today },
            new MyTodo { Title = "Todo 3", IsCompleted = true, CreatedBy = userId, CreatedAt = today.AddDays(-2) }
        };
        TodoContext.MyTodo.AddRange(todos);
        await TodoContext.SaveChangesAsync();

        // Act
        var stats = await TodoRepository.GetStatsAsync(userId);

        // Assert
        stats.ShouldNotBeNull();
        stats.TotalTodos.ShouldBe(3);
        stats.CompletedTodos.ShouldBe(2);
        stats.PendingTodos.ShouldBe(1);
        stats.TodaysCreated.ShouldBe(1);
        stats.TodaysCompleted.ShouldBe(0);
        stats.CompletionRate.ShouldBe(66.7, 0.1); // Allowing a small margin for rounding
    }

    [Fact]
    public async Task GivenNullUserId_GetStatsAsync_ShouldReturnOverallTodoStats()
    {
        // Arrange
        var todos = new List<MyTodo>
        {
            new MyTodo { Title = "Todo 1", IsCompleted = true, CreatedBy = "1", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new MyTodo { Title = "Todo 2", IsCompleted = false, CreatedBy = "2", CreatedAt = DateTime.UtcNow },
            new MyTodo { Title = "Todo 3", IsCompleted = true, CreatedBy = "1", CreatedAt = DateTime.UtcNow }
        };
        TodoContext.MyTodo.AddRange(todos);
        await TodoContext.SaveChangesAsync();

        // Act
        var stats = await TodoRepository.GetStatsAsync();

        // Assert
        stats.ShouldNotBeNull();
        stats.TotalTodos.ShouldBe(3);
        stats.CompletedTodos.ShouldBe(2);
        stats.PendingTodos.ShouldBe(1);
        stats.TodaysCreated.ShouldBe(2);
        stats.TodaysCompleted.ShouldBe(1);
        stats.CompletionRate.ShouldBe(66.7, 0.1); // Allowing a small margin for rounding
    }

}
