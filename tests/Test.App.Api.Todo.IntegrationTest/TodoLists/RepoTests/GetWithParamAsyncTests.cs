using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Microsoft.EntityFrameworkCore.Storage;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoLists.RepoTests;

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
        var todoList = new List<ToDoList>
        {
            new ToDoList { Title = title  , UserId = userId },
            new ToDoList { Title = "Test Lis2", UserId = "2" },
            new ToDoList { Title = "Test List3", UserId = "3" },
            new ToDoList { Title = "Test List4", UserId = "1" }
        };
        TodoContext.ToDoLists.AddRange(todoList);
        TodoContext.SaveChanges();
        var queryParam = new TodoListQueryParam { Title = title, UserId = userId, Page = 1, PageSize = 10 };

        // Act
        var (result, totalItem, totalPage) = await TodoListRepository.GetWithParamAsync(queryParam);

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBeGreaterThan(0);
        result.First().UserId.ShouldBe(userId);
        result.First().Title.ShouldBe(title);
        totalItem.ShouldBe(1);
        totalPage.ShouldBe(1);
    }

    [Fact]
    public async Task GIvenValidModelWithSortBy_GetWithParamAsync_ShouldReturnTodoList()
    {
        // Given
        var todoList = new List<ToDoList>
        {
            new ToDoList { Title = "Test List1", UserId = "1" },
            new ToDoList { Title = "Test List2", UserId = "2" },
            new ToDoList { Title = "Test List3", UserId = "3" },
            new ToDoList { Title = "Test List4", UserId = "4" }
        };
        TodoContext.ToDoLists.AddRange(todoList);
        TodoContext.SaveChanges();
        var queryParam = new TodoListQueryParam { SortBy = "title", SortDirection = "desc", Page = 1, PageSize = 10 };

        // Act
        var (result, totalItem, totalPage) = await TodoListRepository.GetWithParamAsync(queryParam);

        // Assert.
        result.ShouldNotBeNull();
        result.Count().ShouldBeGreaterThan(0);
        result.First().Title.ShouldBe("Test List4");
        result.First().UserId.ShouldBe("4");
        totalItem.ShouldBe(4);
    }

}
