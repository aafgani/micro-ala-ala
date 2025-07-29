using System.Net;
using System.Net.Http.Json;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoLists.EndpointTests;

public class UpdateTests : BaseIntegrationTest
{
    public UpdateTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenTodoListRequest_UpdateTodoList_ShouldReturnOkAsync()
    {
        // Arrange
        var updateListDto = new TodolistDto
        {
            Id = 1,
            Title = "Updated Todo List",
            UserId = "1"
        };
        var todoList = new List<ToDoList>
        {
            new ToDoList { Id = 1, Title = "Test List1", UserId = "1" },
            new ToDoList { Id = 2, Title = "Test List2", UserId = "2" },
            new ToDoList { Id = 3, Title = "Test List3", UserId = "3" },
            new ToDoList { Id = 4, Title = "Test List4", UserId = "4" }
        };
        TodoContext.ToDoLists.AddRange(todoList);
        TodoContext.SaveChanges();
        AuthenticateAsUser("1");

        // Act
        var response = await Client.PutAsJsonAsync("/todos/1", updateListDto);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to update todo list: {errorContent}");
        }

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [MemberData(nameof(UpdateTodoListInvalidData))]
    public async Task GivenInvalidId_UpdateTodoList_ShouldReturnBadRequestAsync(TodolistDto updateListDto, string expectedErrorMessage)
    {
        AuthenticateAsUser("1");

        // Act
        var response = await Client.PutAsJsonAsync("/todos/0", updateListDto);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var errorContent = await response.Content.ReadFromJsonAsync<ApiError>();
        errorContent.Message.ShouldBe(expectedErrorMessage);
    }

    public static IEnumerable<object[]> UpdateTodoListInvalidData =>
        new List<object[]> {
            new object[] { new TodolistDto { Id = 0, Title = "Updated Todo List", UserId = "1" }, "Id must be greater than zero." },
            new object[] { new TodolistDto { Id = 1, Title = "", UserId = "1" }, "Title is required" }
        };
}
