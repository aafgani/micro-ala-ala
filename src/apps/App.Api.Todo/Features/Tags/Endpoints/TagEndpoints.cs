using App.Api.Todo.Features.Tags.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;

namespace App.Api.Todo.Features.Tags.Endpoints
{
    public static class TagEndpoints
    {
        public static RouteGroupBuilder MapTags(this IEndpointRouteBuilder routes)
        {
            var config = routes.ServiceProvider.GetRequiredService<IConfiguration>();
            string pathBase = config["PathBase"] ?? string.Empty;

            var group = routes
                .MapGroup(pathBase + EndpointGroupNames.TagsGroupName)
                .WithTags(EndpointGroupNames.TagsTagName)
                .RequireAuthorization();

            group.MapGet("/", async (ITagService tagService, ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger(nameof(TagEndpoints));
                logger.LogInformation("Fetching all tags");
                var result = await tagService.GetAllAsync();
                return (EndpointResult<IEnumerable<TagDto>, ApiError>)result;
            })
                .WithName("GetAllTags")
                .WithOpenApi();

            return group;
        }
    }
}
