using System;
using System.Net.Http.Json;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoLists;

public class GetByIdTests : BaseIntegrationTest
{
    public GetByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenValidId_GetTodoListById_ShouldReturnOkAsync()
    {
        // Arrange
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
        var response = await Client.GetAsync("/todolists/3");
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get todo list by ID: {errorContent}");
        }

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TodolistDto>();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(3);
        result.Title.ShouldBe("Test List3");
    }
}
