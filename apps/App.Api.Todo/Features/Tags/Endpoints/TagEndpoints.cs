using App.Api.Todo.Features.Tags.Dtos;
using App.Api.Todo.Features.Tags.Services;

namespace App.Api.Todo.Features.Tags.Endpoints
{
    public static class TagEndpoints
    {
        public static RouteGroupBuilder MapTags(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup(EndpointGroupNames.TagsGroupName)
                .WithTags(EndpointGroupNames.TagsTagName);

            group.MapGet("/", async (ITagService tagService) =>
            {
                var result = await tagService.GetAllAsync();
                return Results.Ok(result);
            })
                .WithName("GetAllTags")
                .WithOpenApi();

            return group;
        }
    }
}
