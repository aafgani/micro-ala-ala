namespace App.Api.Todo.Features.Todolist.Endpoints
{
    public static class TodolistEndpoints
    {
        public static RouteGroupBuilder MapTodolist(this IEndpointRouteBuilder routes)
        {
            var group = routes
               .MapGroup(EndpointGroupNames.TodolistsGroupName)
               .WithTags(EndpointGroupNames.TodolistsTagName);



            return group;
        }
    }
}
