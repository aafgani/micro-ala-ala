using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Models;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Task = System.Threading.Tasks.Task;

namespace Test.App.Api.Todo.IntegrationTest.FeatureTests.TagTests.RepositoryTests
{
    public class CreateAsyncTests : BaseIntegrationTest
    {
        public CreateAsyncTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidTagModel_CreateAsync_ShouldCreateTagAsync()
        {
            // Arrange
            var tag = new Tag { Name = "New Tag" };

            // Act
            var createdTag = await TagRepository.CreateAsync(tag);

            // Assert
            var tagsInDb = await TagRepository.GetAllAsync();
            tagsInDb.ShouldNotBeNull();
            tagsInDb.ShouldContain(t => t.Id == createdTag.Id); 
        }
    }
}
