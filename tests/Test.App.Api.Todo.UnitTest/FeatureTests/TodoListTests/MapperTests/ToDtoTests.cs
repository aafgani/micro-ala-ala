using App.Api.Todo.Features.Todolist.Mapper;
using App.Api.Todo.Models;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TodoListTests.MapperTests;

public class ToDtoTests
{
    [Fact]
    public void GivenTodoListEntity_ToDto_ShouldMapCorrectly()
    {
        // Arrange.
        var entity = new ToDoList
        {
            Id = 1,
            Title = "Test List",
            Description = "Test Description",
            CreatedAt = new DateTime(2024, 1, 1),
        };
        var mapper = new TodoListMapper();

        // Act.
        var dto = mapper.ToDto(entity);

        // Assert.
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Title, dto.Title);
        Assert.Equal(entity.Description, dto.Description);
        Assert.Equal(entity.CreatedAt, dto.CreatedAt);
    }
}
