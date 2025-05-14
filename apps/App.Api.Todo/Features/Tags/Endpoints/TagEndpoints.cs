namespace App.Api.Todo.Features.Tags.Endpoints
{
    public static class TagEndpoints
    {
        public static RouteGroupBuilder MapTags(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup(EndpointGroupNames.TagsGroupName)
                .WithTags(EndpointGroupNames.TagsTagName);

            return group;
        }
    }
}
