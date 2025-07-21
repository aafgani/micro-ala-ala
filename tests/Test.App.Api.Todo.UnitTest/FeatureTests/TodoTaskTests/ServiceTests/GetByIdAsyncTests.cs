using App.Api.Todo.Features.Todotask.Data;
using App.Api.Todo.Features.Todotask.Dtos;
using App.Api.Todo.Features.Todotask.Mapper;
using App.Api.Todo.Features.Todotask.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodoTaskServiceTests;

public class GetByIdAsyncTests
{
    [Fact]
    public async Task GivenValidId_GetByIdAsync_ShouldReturnTaskDto_WhenTaskExists()
    {
        // Arrange
        var taskId = 1;
        var mockMapper = new Mock<ITaskMapper>();
        var mockRepository = new Mock<ITodoTaskRepository>();
        var todoTaskService = new TodoTaskService(mockRepository.Object, mockMapper.Object);

        var task = new TodoTask { Id = taskId, Title = "Test Task", DueDate = DateTime.UtcNow };
        mockRepository.Setup(repo => repo.GetByIdWithRelationsAsync(taskId)).ReturnsAsync(task);
        mockMapper.Setup(m => m.ToDto(task)).Returns(new TaskDto { Id = taskId, Title = "Test Task" });

        // Act
        var result = await todoTaskService.GetByIdAsync(taskId);

        // Assert
        result.ShouldBeOfType<Result<TaskDto?, ApiError>>();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(taskId);
    }
}
