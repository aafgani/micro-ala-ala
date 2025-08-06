using App.Api.Todo.Features.Todos.Mapper;
using App.Api.Todo.Models;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodosTests.MapperTests;

public class ToDtoTests
{
    [Fact]
    public void GivenTodoListEntity_ToDto_ShouldMapCorrectly()
    {
        // Arrange.
        var entity = new MyTodo
        {
            Id = 1,
            Title = "Test Todo",
            Notes = "Test Description",
            CreatedAt = new DateTime(2024, 1, 1),
            AssignTo = "user123",
            IsCompleted = false,
            DueDate = new DateTime(2024, 1, 10),
            CreatedBy = "creator123"
        };

        var mapper = new TodoMapper();

        // Act.
        var dto = mapper.ToDto(entity);

        // Assert.
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Title, dto.Title);
        Assert.Equal(entity.Notes, dto.Description);
        Assert.Equal(entity.CreatedAt, dto.CreatedAt);
        Assert.Equal(entity.AssignTo, dto.UserId);
        Assert.Equal(entity.IsCompleted, dto.IsCompleted);
        Assert.Equal(entity.DueDate, dto.DueDate);
        Assert.Equal(entity.CreatedBy, dto.CreatedBy);
    }
}
