using App.Api.Todo.Features.Todos.Mapper;
using App.Common.Domain.Dtos.Todo;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodosTests.MapperTests;

public class ToEntityTests
{
    [Fact]
    public void GivenTodoDto_ToModel_ShouldMapCorrectly()
    {
        // Arrange.
        var dto = new TodolistDto
        {
            Id = 1,
            Title = "Test Todo",
            Description = "Test Description",
            CreatedAt = new DateTime(2024, 1, 1),
            UserId = "user123",
            IsCompleted = true,
            DueDate = new DateTime(2024, 1, 10),
            CreatedBy = "creator123"
        };

        var mapper = new TodoMapper();

        // Act.
        var entity = mapper.ToEntity(dto);

        // Assert.
        Assert.Equal(dto.Id, entity.Id);
        Assert.Equal(dto.Title, entity.Title);
        Assert.Equal(dto.Description, entity.Notes);
        Assert.Equal(dto.CreatedAt, entity.CreatedAt);
        Assert.Equal(dto.UserId, entity.AssignTo);
        Assert.Equal(dto.IsCompleted, entity.IsCompleted);
        Assert.Equal(dto.DueDate, entity.DueDate);
        Assert.Equal(dto.CreatedBy, entity.CreatedBy);
    }
}
