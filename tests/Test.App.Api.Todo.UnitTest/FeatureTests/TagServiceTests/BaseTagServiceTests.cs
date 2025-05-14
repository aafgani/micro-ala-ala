using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Features.Tags.Mapper;
using Moq;

namespace Test.App.Api.Todo.UnitTest.FeatureTests.TagServiceTests
{
    public class BaseTagServiceTests
    {
        protected (Mock<ITagRepository> mockTagRepository, Mock<ITagMapper> mockTagMapper) GetDependencies()
        {
            var mockTagRepository = new Mock<ITagRepository>();
            var mockTagMapper = new Mock<ITagMapper>();
            return (mockTagRepository, mockTagMapper);
        }
    }
}
