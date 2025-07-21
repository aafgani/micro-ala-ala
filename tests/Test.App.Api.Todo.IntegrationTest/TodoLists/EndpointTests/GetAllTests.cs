using System.Net.Http.Json;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoLists.EndpointTests;

public class GetAllTests : BaseIntegrationTest
{
    public GetAllTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenValidRequest_GetAllTodoLists_ShouldReturnOkAsync()
    {
        // Arrange
        var userId = "1"; // Assuming a user with ID 1 exists
        var title = "Test List 1";
        var param = new TodoListQueryParam
        {
            Page = 1,
            PageSize = 10,
            UserId = "1"
        };
        var todoList = new List<ToDoList>
        {
            new ToDoList { Title = title  , UserId = userId },
            new ToDoList { Title = "Test Lis2", UserId = "2" },
            new ToDoList { Title = "Test List3", UserId = "3" },
            new ToDoList { Title = "Test List4", UserId = userId }
        };
        TodoContext.ToDoLists.AddRange(todoList);
        TodoContext.SaveChanges();
        AuthenticateAsUser("1");

        // Act
        var response = await Client.GetAsync($"/todos?userId={param.UserId}&page={param.Page}&pageSize={param.PageSize}");

        // Assert.
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get todo lists: {errorContent}");
        }
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TodolistDto>>();
        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(2); // Assuming we only want lists for userId "
    }


    [Fact(Skip = "User validation not implemented yet - cannot verify if user exists")]
    public async Task GivenInvalidUserId_GetAllTodoLists_ShouldReturnNotFoundAsync()
    {
        // Arrange
        var userId = "999"; // Assuming no user with ID 999 exists
        var param = new TodoListQueryParam
        {
            Page = 1,
            PageSize = 10,
            UserId = userId
        };
        AuthenticateAsUser("1");

        // Act
        var response = await Client.GetAsync($"/todos?userId={param.UserId}&page={param.Page}&pageSize={param.PageSize}");

        // Assert.
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GivenNoTodoLists_GetAllTodoLists_ShouldReturnEmptyPagedResultAsync()
    {
        // Arrange
        TodoContext.ToDoLists.RemoveRange(TodoContext.ToDoLists);
        TodoContext.SaveChanges();
        var userId = "1"; // Assuming a user with ID 1 exists
        var param = new TodoListQueryParam
        {
            Page = 1,
            PageSize = 10,
            UserId = userId
        };
        AuthenticateAsUser("1");

        // Act
        var response = await Client.GetAsync($"/todos?userId={param.UserId}&page={param.Page}&pageSize={param.PageSize}");

        // Assert.
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TodolistDto>>();
        result.ShouldNotBeNull();
        result.Data.ShouldBeEmpty();
    }

    [Fact]
    public async Task GivenInvalidQueryParams_GetAllTodoLists_ShouldReturnBadRequestAsync()
    {
        // Arrange
        var userId = "1"; // Assuming a user with ID 1 exists
        AuthenticateAsUser("1");

        // Act
        var response = await Client.GetAsync($"/todos?userId={userId}&page=invalid&pageSize=10");

        // Assert.
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }
}
