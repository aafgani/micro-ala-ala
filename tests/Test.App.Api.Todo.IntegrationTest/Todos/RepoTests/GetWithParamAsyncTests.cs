using System;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Microsoft.EntityFrameworkCore.Storage;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.Todos.RepoTests;

public class GetWithParamAsyncTests : BaseIntegrationTest, IAsyncLifetime
{
    private IDbContextTransaction _transaction;

    public GetWithParamAsyncTests(IntegrationTestWebAppFactory factory) : base(factory)
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
    public async Task GIvenValidTodoListId_GetWithParamAsync_ShouldReturnTodoList()
    {
        // Arrange
        var title = "Test List 1";
        var userId = "1";
        var todoList = new List<MyTodo>
        {
            new MyTodo { Title = title  , AssignTo = userId, CreatedBy = userId },
            new MyTodo { Title = "Test Lis2", AssignTo = "2", CreatedBy = "2" },
            new MyTodo { Title = "Test List3", AssignTo = "3", CreatedBy = "3" },
            new MyTodo { Title = "Test List4", AssignTo = "1", CreatedBy = "1" }
        };
        TodoContext.MyTodo.AddRange(todoList);
        TodoContext.SaveChanges();
        var queryParam = new TodoListQueryParam { Title = title, UserId = userId, Page = 1, PageSize = 10 };

        // Act
        var (result, totalItem, totalPage) = await TodoRepository.GetWithParamAsync(queryParam);

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBeGreaterThan(0);
        result.First().AssignTo.ShouldBe(userId);
        result.First().Title.ShouldBe(title);
        totalItem.ShouldBe(1);
        totalPage.ShouldBe(1);
    }

    [Fact]
    public async Task GIvenValidModelWithSortBy_GetWithParamAsync_ShouldReturnTodoList()
    {
        // Arrange
        var title = "Test List 1";
        var userId = "1";
        var todoList = new List<MyTodo>
        {
            new MyTodo { Title = title  , AssignTo = userId, CreatedBy = userId },
            new MyTodo { Title = "Test Lis2", AssignTo = "2", CreatedBy = "2" },
            new MyTodo { Title = "Test List3", AssignTo = "3", CreatedBy = "3" },
            new MyTodo { Title = "Test List4", AssignTo = "1", CreatedBy = "1" }
        };
        TodoContext.MyTodo.AddRange(todoList);
        TodoContext.SaveChanges();
        var queryParam = new TodoListQueryParam { Title = title, UserId = userId, Page = 1, PageSize = 10, SortBy = "title", SortDirection = "asc" };

        // Act
        var (result, totalItem, totalPage) = await TodoRepository.GetWithParamAsync(queryParam);

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBeGreaterThan(0);
        result.First().AssignTo.ShouldBe(userId);
        result.First().Title.ShouldBe(title);
        totalItem.ShouldBe(1);
        totalPage.ShouldBe(1);
    }

}
