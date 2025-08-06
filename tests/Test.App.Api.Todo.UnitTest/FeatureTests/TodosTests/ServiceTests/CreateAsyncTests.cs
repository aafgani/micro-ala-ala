using App.Api.Todo.Features.Todos.Data;
using App.Api.Todo.Features.Todos.Mapper;
using App.Api.Todo.Features.Todos.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodosTests.ServiceTests;

public class CreateAsyncTests
{
    [Fact]
    public void GivenTodoListDto_CreateAsync_ShouldMapCorrectly()
    {
        // Arrange
        var dto = new TodolistDto
        {
            Id = 1,
            Title = "Test List",
            IsCompleted = false,
            UserId = "TestUser",
            DueDate = new DateTime(2024, 1, 1),
            Description = "Test Description",
            CreatedAt = new DateTime(2024, 1, 1),
            CreatedBy = "TestUser",
        };
        var todolistModel = new MyTodo
        {
            Id = dto.Id,
            Title = dto.Title,
            Notes = dto.Description,
            CreatedAt = dto.CreatedAt,
            CreatedBy = dto.CreatedBy,
            IsCompleted = dto.IsCompleted,
            AssignTo = dto.UserId,
            DueDate = dto.DueDate
        };
        var mockRepository = new Mock<ITodoRepository>();
        var mockMapper = new Mock<ITodoMapper>();
        mockMapper.Setup(m => m.ToEntity(It.Is<TodolistDto>(m => m.Id == dto.Id &&
                                                                    m.Title == dto.Title &&
                                                                    m.Description == dto.Description &&
                                                                    m.CreatedAt == dto.CreatedAt &&
                                                                    m.CreatedBy == dto.CreatedBy &&
                                                                    m.IsCompleted == dto.IsCompleted &&
                                                                    m.UserId == dto.UserId &&
                                                                    m.DueDate == dto.DueDate)))
                                .Returns(todolistModel);
        mockMapper.Setup(m => m.ToDto(It.Is<MyTodo>(m => m.Id == todolistModel.Id &&
                                                            m.Title == todolistModel.Title &&
                                                            m.Notes == todolistModel.Notes &&
                                                            m.CreatedAt == todolistModel.CreatedAt &&
                                                            m.CreatedBy == todolistModel.CreatedBy &&
                                                            m.IsCompleted == todolistModel.IsCompleted &&
                                                            m.AssignTo == todolistModel.AssignTo &&
                                                            m.DueDate == todolistModel.DueDate)))
                                .Returns(dto);
        mockRepository.Setup(r => r.CreateAsync(It.Is<MyTodo>(m => m.Id == todolistModel.Id &&
                                                                    m.Title == todolistModel.Title &&
                                                                    m.Notes == todolistModel.Notes &&
                                                                    m.CreatedAt == todolistModel.CreatedAt &&
                                                                    m.CreatedBy == todolistModel.CreatedBy &&
                                                                    m.IsCompleted == todolistModel.IsCompleted &&
                                                                    m.AssignTo == todolistModel.AssignTo &&
                                                                    m.DueDate == todolistModel.DueDate)))
                                    .ReturnsAsync(todolistModel);
        var service = new TodoService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = service.CreateAsync(dto).Result;

        // Assert
        result.ShouldBeOfType<Result<TodolistDto, ApiError>>();
        result.Value.Id.ShouldBe(dto.Id);
        result.Value.Title.ShouldBe(dto.Title);
        result.Value.Description.ShouldBe(dto.Description);
        result.Value.CreatedAt.ShouldBe(dto.CreatedAt);
        result.Value.CreatedBy.ShouldBe(dto.CreatedBy);
        result.Value.IsCompleted.ShouldBe(dto.IsCompleted);
        result.Value.UserId.ShouldBe(dto.UserId);
        result.Value.DueDate.ShouldBe(dto.DueDate);
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBeGreaterThan(0);

    }

}
