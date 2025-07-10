using App.Common.Domain.Dtos.Todo;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoLists.EndpointTests;

public class GetAllTests : BaseIntegrationTest
{
    public GetAllTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenValidRequest_GetAllTodoLists_ShouldReturnOkAsync()
    {
        // Arrange
        var userId = "1"; // Assuming a user with ID 1 exists
        var param = new TodoListQueryParam
        {
            Page = 1,
            PageSize = 10,
            UserId = "1"
        };

        // Act

    }
}
