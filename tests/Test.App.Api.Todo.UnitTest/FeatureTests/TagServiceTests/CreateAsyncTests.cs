using App.Api.Todo.Features.Tags.Dtos;
using App.Api.Todo.Features.Tags.Services;
using App.Api.Todo.Models;
using Moq;
using Shouldly;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TagServiceTests
{
    public class CreateAsyncTests : BaseTagServiceTests
    {
        [Fact]
        public async System.Threading.Tasks.Task GivenValidDto_CreateAsync_ShouldCreateTagAsync()
        {
            // Arrange.
            var createDto = new TagDto { Name = "TestTag" };
            var newTag = new Tag { Id = 1, Name = "TestTag" };
            var (mockTagRepository, mockMapper) = GetDependencies();
            mockTagRepository.Setup(repo => repo.CreateAsync(It.IsAny<Tag>()))
                .ReturnsAsync(newTag); 
            mockMapper.Setup(mapper => mapper.ToEntity(It.IsAny<TagDto>())).Returns(newTag);
            mockMapper.Setup(mapper => mapper.ToDto(It.IsAny<Tag>())).Returns(createDto);
            var tagService = new TagService(mockTagRepository.Object, mockMapper.Object);

            // Act.
            var result = await tagService.CreateAsync(createDto);

            // Assert.
            mockTagRepository.Verify(repo => repo.CreateAsync(It.Is<Tag>(t => t.Name.Equals(createDto.Name))), Times.Once);
            result.ShouldNotBeNull();
            result.Name.ShouldBe(createDto.Name);
        }
    }
}
