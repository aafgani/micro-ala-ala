using App.Api.Todo.Features.Tags.Services;
using App.Api.Todo.Models;
using Moq;
using Shouldly;
using Task = System.Threading.Tasks.Task;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TagServiceTests
{
    public class DeleteAsyncTests : BaseTagServiceTests
    {
        [Fact]
        public async Task GivenValidId_DeleteAsync_ShouldDeleteTag()
        {
            // Arrange.
            var tagId = 1;
            var tag = new Tag { Id = tagId, Name = "TestTag" };
            var (mockTagRepository, mockMapper, mockTagLogger) = GetDependencies();
            mockTagRepository.Setup(repo => repo.GetByIdAsync(tagId)).ReturnsAsync(tag);
            mockTagRepository.Setup(repo => repo.DeleteAsync(tag)).Returns(Task.CompletedTask);
            var tagService = new TagService(mockTagRepository.Object, mockMapper.Object, mockTagLogger.Object);

            // Act.
            var result = await tagService.DeleteAsync(tagId);

            // Assert.
            mockTagRepository.Verify(repo => repo.GetByIdAsync(tagId), Times.Once);
            mockTagRepository.Verify(repo => repo.DeleteAsync(tag), Times.Once);
            result.Value.ShouldBeTrue();
        }
    }
}
