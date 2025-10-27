using App.Api.Todo.Features.Tags.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TagServiceTests
{
    public class GetByIdAsyncTests : BaseTagServiceTests
    {
        [Fact]
        public void GivenValidId_GetByIdAsync_ShouldReturnTag()
        {
            // Arrange.
            var tagId = 1;
            var tag = new Tag { Id = tagId, Name = "TestTag" };
            var (mockTagRepository, mockMapper, mockTagLogger) = GetDependencies();
            mockTagRepository.Setup(repo => repo.GetByIdAsync(tagId)).ReturnsAsync(tag);
            mockMapper.Setup(mapper => mapper.ToDto(tag)).Returns(new TagDto { Name = tag.Name });
            var tagService = new TagService(mockTagRepository.Object, mockMapper.Object, mockTagLogger.Object);

            // Act.
            var result = tagService.GetByIdAsync(tagId).Result;

            // Assert.
            mockTagRepository.Verify(repo => repo.GetByIdAsync(tagId), Times.Once);
            result.Value.ShouldNotBeNull();
            result.Value.Name.ShouldBe(tag.Name);
        }
    }
}
