using App.Api.Todo.Models;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoLists.RepoTests;

public class CreateAsyncTests : BaseIntegrationTest
{
    public CreateAsyncTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenValidTodoListModel_CreateAsync_ShouldCreateTodoListAsync()
    {
        // Arrange
        var todoList = new ToDoList { Title = "Test List", UserId = "1" };

        // Act
        var createdTodoList = await TodoListRepository.CreateAsync(todoList);
        await TodoContext.SaveChangesAsync();

        // Assert
        var listsInDb = TodoContext.ToDoLists.Where(i => i.Id == createdTodoList.Id).FirstOrDefault();
        listsInDb.ShouldNotBeNull();
        listsInDb.Title.ShouldBe(todoList.Title);
    }
}
