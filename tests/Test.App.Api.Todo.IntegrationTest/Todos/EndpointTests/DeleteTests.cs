using App.Api.Todo.Models;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.Todos.EndpointTests;

public class DeleteTests : BaseIntegrationTest
{
    public DeleteTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenValidId_DeleteTodo_ShouldReturnOkAsync()
    {
        // Arrange
        var todoList = new List<MyTodo>
        {
            new MyTodo { Id = 1, Title = "Test List1", CreatedBy = "1" },
            new MyTodo { Id = 2, Title = "Test List2", CreatedBy = "2" },
            new MyTodo { Id = 3, Title = "Test List3", CreatedBy = "3" },
            new MyTodo { Id = 4, Title = "Test List4", CreatedBy = "4" }
        };
        TodoContext.MyTodo.AddRange(todoList);
        TodoContext.SaveChanges();
        AuthenticateAsUser("1");

        // Act
        var response = await Client.DeleteAsync("/todos/1");
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete todo list: {errorContent}");
        }

        // Assert
        response.EnsureSuccessStatusCode();
        TodoContext.ChangeTracker.Clear();
        var deletedTodo = TodoContext.MyTodo.Find(1);
        deletedTodo.ShouldBeNull();
        var remainingCount = await TodoContext.MyTodo.CountAsync();
        remainingCount.ShouldBe(3);
    }
}
