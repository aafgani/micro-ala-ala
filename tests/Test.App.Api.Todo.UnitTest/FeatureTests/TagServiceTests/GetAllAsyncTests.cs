﻿using App.Api.Todo.Features.Tags.Dtos;
using App.Api.Todo.Features.Tags.Services;
using App.Api.Todo.Models;
using Moq;
using Shouldly;
using Task = System.Threading.Tasks.Task;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TagServiceTests
{
    public class GetAllAsyncTests : BaseTagServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTags()
        {
            // Arrange
            var tags = new List<Tag>
            {
                new Tag { Id = 1, Name = "Tag1" },
                new Tag { Id = 2, Name = "Tag2" }
            };
            var (mockTagRepository, mockMapper) = GetDependencies();
            mockTagRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tags);
            mockMapper.Setup(mapper => mapper.ToDto(It.IsAny<Tag>()))
                .Returns((Tag tag) => new TagDto { Name = tag.Name }); 
            var tagService = new TagService(mockTagRepository.Object, mockMapper.Object);

            // Act
            var result = await tagService.GetAllAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.First().Name.ShouldBe("Tag1");   
        }
    }
}
