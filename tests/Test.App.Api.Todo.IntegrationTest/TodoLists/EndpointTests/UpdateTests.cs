using System;
using System.Net.Http.Json;
using App.Api.Todo.Models;
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
}
