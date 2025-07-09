using App.Api.Todo.Features.Todolist.Data;
using App.Api.Todo.Features.Todolist.Mapper;
using App.Api.Todo.Features.Todolist.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Moq;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodoListTests.ServiceTests;

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
            Description = "Test Description",
            CreatedAt = new DateTime(2024, 1, 1),
        };
        var todolistModel = new ToDoList
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = dto.CreatedAt
        };
        var mockRepository = new Mock<ITodolistRepository>();
        var mockMapper = new Mock<ITodoListMapper>();

        mockMapper.Setup(m => m.ToEntity(It.Is<TodolistDto>(m => m.Id == dto.Id &&
                                                                    m.Title == dto.Title &&
                                                                    m.Description == dto.Description &&
                                                                    m.CreatedAt == dto.CreatedAt)))
                                .Returns(todolistModel);

        mockMapper.Setup(m => m.ToDto(It.Is<ToDoList>(m => m.Id == todolistModel.Id &&
                                                            m.Title == todolistModel.Title &&
                                                            m.Description == todolistModel.Description &&
                                                            m.CreatedAt == todolistModel.CreatedAt)))
                                .Returns(dto);

        mockRepository.Setup(r => r.CreateAsync(It.Is<ToDoList>(m => m.Id == todolistModel.Id &&
                                                                    m.Title == todolistModel.Title &&
                                                                    m.Description == todolistModel.Description &&
                                                                    m.CreatedAt == todolistModel.CreatedAt)))
                                    .ReturnsAsync(todolistModel);

        var service = new TodolistService(mockRepository.Object, mockMapper.Object);

        // Act
        var result = service.CreateAsync(dto).Result;

        // Assert
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.CreatedAt, result.CreatedAt);
    }
}
