using App.Api.Todo.Features.Todotask.Data;
using App.Api.Todo.Features.Todotask.Mapper;
using App.Api.Todo.Features.Todotask.Services;
using App.Api.Todo.Models;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodoTaskTests.ServiceTests;

public class DeleteAsyncTests
{
    [Fact]
    public async Task GivenTodoTasks_DeleteAsync_ShouldDelete()
    {
        // Arrange
        var todoTaskRepository = new Mock<ITodoTaskRepository>();
        todoTaskRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new TodoTask { Id = 1, Title = "Test Task" });
        todoTaskRepository.Setup(repo => repo.DeleteAsync(It.IsAny<TodoTask>()))
            .Returns(Task.CompletedTask);
        var mapper = Mock.Of<ITaskMapper>();
        var todoTaskService = new TodoTaskService(todoTaskRepository.Object, mapper);

        // Act
        var result = await todoTaskService.DeleteAsync(1);

        // Assert
        result.Value.ShouldBeTrue();
    }
}
