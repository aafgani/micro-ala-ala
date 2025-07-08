using App.Api.Todo.Features.Tags.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos;
using Moq;
using Shouldly;
using Task = System.Threading.Tasks.Task;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TagServiceTests
{
    public class UpdateAsyncTests : BaseTagServiceTests
    {
        [Fact]
        public void GivenValidId_UpdateAsync_ShouldReturnTrue()
        {
            // Arrange.
            var tagId = 1;
            var tagDto = new TagDto { Name = "UpdatedTag" };
            var tag = new Tag { Id = tagId, Name = "TestTag" };
            var (mockTagRepository, mockMapper) = GetDependencies();
            mockTagRepository.Setup(repo => repo.GetByIdAsync(tagId)).ReturnsAsync(tag);
            mockMapper.Setup(mapper => mapper.ToEntity(tagDto)).Returns(tag);
            mockTagRepository.Setup(repo => repo.UpdateAsync(tag)).Returns(Task.FromResult(1));
            var tagService = new TagService(mockTagRepository.Object, mockMapper.Object);

            // Act.
            var result = tagService.UpdateAsync(tagId, tagDto).Result;

            // Assert.
            mockTagRepository.Verify(repo => repo.GetByIdAsync(tagId), Times.Once);
            mockTagRepository.Verify(repo => repo.UpdateAsync(tag), Times.Once);
            result.ShouldBeTrue();
        }
    }
}
