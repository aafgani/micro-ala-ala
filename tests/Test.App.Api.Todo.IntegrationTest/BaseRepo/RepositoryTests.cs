using App.Api.Todo.Models;
using App.Common.Infrastructure.Model;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.BaseRepo;

public class RepoTests : BaseIntegrationTest
{
    private readonly Repository<ToDoList> _repository;
    public RepoTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _repository = new Repository<ToDoList>(TodoContext);
    }

    [Fact]
    public async Task GivenValidEntity_CreateAsync_ShouldAddEntity()
    {
        // Arrange
        var entity = new ToDoList { Title = "Test List", UserId = "1" };

        // Act
        var createdEntity = await _repository.CreateAsync(entity);

        // Assert
        createdEntity.ShouldNotBeNull();
        createdEntity.Id.ShouldBeGreaterThan(0);
        createdEntity.Title.ShouldBe("Test List");
    }

    [Fact]
    public async Task GivenExistingEntity_GetByIdAsync_ShouldReturnEntity()
    {
        // Arrange
        var entity = new ToDoList { Title = "GetById List", UserId = "2" };
        var createdEntity = await _repository.CreateAsync(entity);

        // Act
        var fetchedEntity = await _repository.GetByIdAsync(createdEntity.Id);

        // Assert
        fetchedEntity.ShouldNotBeNull();
        fetchedEntity.Id.ShouldBe(createdEntity.Id);
        fetchedEntity.Title.ShouldBe("GetById List");
    }

    [Fact]
    public async Task GivenExistingEntity_UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var entity = new ToDoList { Title = "Original Title", UserId = "3" };
        var createdEntity = await _repository.CreateAsync(entity);

        // Act
        createdEntity.Title = "Updated Title";
        var updatedEntity = await _repository.UpdateAsync(createdEntity);

        // Assert
        updatedEntity.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GivenExistingEntity_DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var entity = new ToDoList { Title = "To Delete", UserId = "4" };
        var createdEntity = await _repository.CreateAsync(entity);

        // Act
        await _repository.DeleteAsync(entity);

        // Assert
        await Should.ThrowAsync<Exception>(async () =>
        {
            await _repository.GetByIdAsync(createdEntity.Id);
        });
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        await _repository.CreateAsync(new ToDoList { Title = "List 1", UserId = "5" });
        await _repository.CreateAsync(new ToDoList { Title = "List 2", UserId = "6" });

        // Act
        var allEntities = await _repository.GetAllAsync();

        // Assert
        allEntities.ShouldNotBeNull();
        allEntities.Count().ShouldBeGreaterThanOrEqualTo(2);
    }
}
