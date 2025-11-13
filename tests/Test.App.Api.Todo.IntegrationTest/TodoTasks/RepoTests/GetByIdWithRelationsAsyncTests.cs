using System;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoTasks.RepoTests;

public class GetByIdWithRelationsAsyncTests : BaseIntegrationTest
{
    public GetByIdWithRelationsAsyncTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetByIdWithRelationsAsync_ReturnsEntityWithRelations_WhenExists()
    {
        // Arrange
        // TODO: seed a TodoTask with InverseParentTask and Tags into _db and SaveChangesAsync
        //  var todoList = new ToDoList { Title = "Test List", UserId = "1" };
        //     var tags = new List<Tag> { new Tag { Name = "Urgent" } };

        // Act
        // var result = await _repo.GetByIdWithRelationsAsync(seedId);

        // Assert
        // TODO: assert result is not null and related navigation properties are loaded
    }

    [Fact]
    public async Task GetByIdWithRelationsAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        // ensure DB empty or id not present

        // Act
        // var result = await _repo.GetByIdWithRelationsAsync(9999);

        // Assert
        // Assert.Null(result);
    }
}
