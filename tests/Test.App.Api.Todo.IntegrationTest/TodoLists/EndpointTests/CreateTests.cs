using System.Net.Http.Json;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoLists.EndpointTests;

public class CreateTests : BaseIntegrationTest
{
    public CreateTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenTodoListRequest_CreateTodoList_ShouldReturnOkAsync()
    {
        // Arrange
        var createListDto = new TodolistDto
        {
            Title = "New Todo List",
            UserId = "1"
        };
        AuthenticateAsUser("1");

        // Act
        var response = await Client.PostAsJsonAsync("/todos", createListDto);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create todo list: {errorContent}");
        }

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TodolistDto>();
        result.ShouldNotBeNull();
    }
}
