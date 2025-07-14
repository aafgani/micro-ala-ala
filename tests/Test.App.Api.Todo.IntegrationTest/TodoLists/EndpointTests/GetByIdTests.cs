using System.Net;
using System.Net.Http.Json;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoLists.EndpointTests;

public class GetByIdTests : BaseIntegrationTest
{
    public GetByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenValidId_GetTodoListById_ShouldReturnOkAsync()
    {
        // Arrange
        var userId = "1";
        var todoList = new List<ToDoList>
    {
        new ToDoList { Id = 1, Title = "Test List1", UserId = "1" },
        new ToDoList { Id = 2, Title = "Test List2", UserId = "2" },
        new ToDoList { Id = 3, Title = "Test List3", UserId = "3" },
        new ToDoList { Id = 4, Title = "Test List4", UserId = "4" }
    };
        TodoContext.ToDoLists.AddRange(todoList);
        TodoContext.SaveChanges();

        // ✅ Explicitly authenticate this test
        AuthenticateAsUser(userId);

        // 🔍 Debug: Check the authorization header
        var authHeader = Client.DefaultRequestHeaders.Authorization;
        Console.WriteLine($"Auth Header: {authHeader?.Scheme} {authHeader?.Parameter}");

        // Act
        var response = await Client.GetAsync("/todos/3");

        // 🔍 Debug: Check response details if failed
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {errorContent}");
            Console.WriteLine($"Response Headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))}");
        }

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TodolistDto>();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(3);
        result.Title.ShouldBe("Test List3");
    }

    [Fact]
    public async Task GivenUnauthenticatedRequest_GetTodoListById_ShouldReturnUnauthorized()
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/todos");

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }
}
