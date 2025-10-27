using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Features.Tags.Mapper;
using App.Api.Todo.Features.Tags.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TagServiceTests
{
    public class BaseTagServiceTests
    {
        protected (Mock<ITagRepository> mockTagRepository, Mock<ITagMapper> mockTagMapper, Mock<ILogger<TagService>> mockTagLogger) GetDependencies()
        {
            var mockTagRepository = new Mock<ITagRepository>();
            var mockTagMapper = new Mock<ITagMapper>();
            var mockTagLogger = new Mock<ILogger<TagService>>();
            return (mockTagRepository, mockTagMapper, mockTagLogger);
        }
    }
}
