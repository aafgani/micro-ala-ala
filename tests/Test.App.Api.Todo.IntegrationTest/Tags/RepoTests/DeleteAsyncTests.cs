using App.Api.Todo.Models;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;
using Task = System.Threading.Tasks.Task;

namespace Test.App.Todo.Integration.Tags.RepoTests
{
    public class DeleteAsyncTests : BaseIntegrationTest
    {
        public DeleteAsyncTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidModel_DeleteAsync_ShouldDeleteTag()
        {
            // Arrange
            var tag = new Tag { Name = "New Tag" };
            await TagRepository.CreateAsync(tag);

            // Act & Assert
            
            await TagRepository.DeleteAsync(tag);

            // Assert
            var tagsInDb = await TagRepository.GetAllAsync();
            tagsInDb.ShouldNotContain(tag);
        }
    }
}
