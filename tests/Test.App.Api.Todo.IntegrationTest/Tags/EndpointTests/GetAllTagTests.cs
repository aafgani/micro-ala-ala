﻿using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Azure;
using Shouldly;
using System.Net.Http.Json;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;
using Task = System.Threading.Tasks.Task;

namespace Test.App.Todo.Integration.Tags.EndpointTests
{
    public class GetAllTagTests : BaseIntegrationTest
    {
        public GetAllTagTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidRequest_GetAllTags_ShouldReturnOk()
        {
            // Arrange
            var tags = new List<Tag>()
            {
                new Tag { Name = "Tag 1" },
                new Tag { Name = "Tag 2" },
                new Tag { Name = "Tag 3" }
            };
            foreach (var tag in tags)
            {
                await TagRepository.CreateAsync(tag);
            }
            AuthenticateAsUser("1");

            // Act
            var response = await Client.GetAsync("/tags");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<TagDto>>();
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            result.First().Name.ShouldBe("Tag 1");
        }
    }
}
