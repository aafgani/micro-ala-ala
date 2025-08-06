using System;
using App.Api.Todo.Features.Todos.Data;
using App.Api.Todo.Features.Todos.Mapper;
using App.Api.Todo.Features.Todos.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodosTests.ServiceTests;

public class UpdateAsyncTests
{
    [Fact]
    public async Task UpdateAsync_ShouldUpdateTodoList_WhenTodoListExists()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<ITodoMapper>();
        var service = new TodoService(mockRepository.Object, mockMapper.Object);

        var existingTodoList = new MyTodo { Id = 1, Title = "Old Title", AssignTo = "User1", DueDate = DateTime.UtcNow.AddDays(7), IsCompleted = false, Notes = "Some notes" };
        var updateDto = new TodolistDto { Id = 1, Title = "New Title", UserId = "User2", DueDate = DateTime.UtcNow.AddDays(10), IsCompleted = true, Description = "Updated notes" };

        mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingTodoList);

        // Act
        var result = await service.UpdateAsync(1, updateDto);

        // Assert
        result.ShouldBeOfType<Result<bool, ApiError>>();
        result.Value.ShouldBeTrue();
        mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<MyTodo>()), Times.Once);
        mockRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        mockMapper.VerifyNoOtherCalls();
    }
}
