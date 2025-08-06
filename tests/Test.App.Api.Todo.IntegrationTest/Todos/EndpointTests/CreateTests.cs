using System;
using System.Net.Http.Json;
using App.Common.Domain.Dtos.Todo;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.Todos.EndpointTests;

public class CreateTests : BaseIntegrationTest
{
    public CreateTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenTodoListRequest_CreateTodo_ShouldReturnOkAsync()
    {
        // Arrange
        var createListDto = new TodolistDto
        {
            Title = "New Todo List",
            Description = "Description for new todo list",
            IsCompleted = false,
            UserId = "1",
            DueDate = DateTime.UtcNow.AddDays(7), // Example due date
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "User Test"
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
        result.Title.ShouldBe(createListDto.Title);
        result.Description.ShouldBe(createListDto.Description);
        result.IsCompleted.ShouldBe(createListDto.IsCompleted);
        result.UserId.ShouldBe(createListDto.UserId);
        result.DueDate.ShouldBe(createListDto.DueDate);
        result.CreatedAt.ShouldBe(createListDto.CreatedAt);
        result.CreatedBy.ShouldBe(createListDto.CreatedBy);
    }

}
