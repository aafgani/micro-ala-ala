using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoTasks.RepoTests;

public class GetWithParamAsyncTests : BaseIntegrationTest
{
    public GetWithParamAsyncTests(IntegrationTestWebAppFactory factory) : base(factory)
    {

    }

    [Fact]
    public async Task GetWithParamAsync_FiltersByDueDate()
    {
        // Arrange
        // TODO: seed tasks with different DueDate values

        // Act
        // var param = new TodoTaskQueryParam { DueDate = someDate, Page = 1, PageSize = 10 };
        // var (items, totalItems, totalPages) = await _repo.GetWithParamAsync(param);

        // Assert
        // TODO: assert all returned items have the specified DueDate and totals match
    }

    [Theory]
    [InlineData("title", "asc")]
    [InlineData("title", "desc")]
    [InlineData("duedate", "asc")]
    [InlineData("duedate", "desc")]
    public async Task GetWithParamAsync_SortsAccordingToParameters(string sortBy, string sortDirection)
    {
        // Arrange
        // TODO: seed multiple tasks with varying Title and DueDate

        // Act
        // var param = new TodoTaskQueryParam { SortBy = sortBy, SortDirection = sortDirection, Page = 1, PageSize = 10 };
        // var (items, _, _) = await _repo.GetWithParamAsync(param);

        // Assert
        // TODO: assert items are ordered as expected depending on sortBy/sortDirection
    }

    [Fact]
    public async Task GetWithParamAsync_UsesDefaultSort_WhenInvalidSortBy()
    {
        // Arrange
        // TODO: seed tasks

        // Act
        // var param = new TodoTaskQueryParam { SortBy = "invalid", SortDirection = "asc", Page = 1, PageSize = 10 };
        // var (items, _, _) = await _repo.GetWithParamAsync(param);

        // Assert
        // TODO: assert default sort (by Title) was applied
    }

    [Fact]
    public async Task GetWithParamAsync_Pagination_ReturnsCorrectPageInfo()
    {
        // Arrange
        // TODO: seed e.g. 25 tasks

        // Act
        // var param = new TodoTaskQueryParam { Page = 2, PageSize = 10 };
        // var (items, totalItems, totalPages) = await _repo.GetWithParamAsync(param);

        // Assert
        // TODO: assert items.Count == 10, totalItems == 25, totalPages == 3
    }

    [Fact]
    public async Task GetWithParamAsync_PageOutOfRange_ReturnsEmptyCollection()
    {
        // Arrange
        // TODO: seed small number of tasks (e.g. 5)

        // Act
        // var param = new TodoTaskQueryParam { Page = 10, PageSize = 10 };
        // var (items, totalItems, totalPages) = await _repo.GetWithParamAsync(param);

        // Assert
        // TODO: assert items is empty and totalItems/totalPages reflect seeded data
    }
}
