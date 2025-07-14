using App.Api.Todo.Features.Todolist.Services;
using App.Common.Domain.Dtos.Todo;

namespace App.Api.Todo.Features.Todolist.Endpoints
{
    public static class TodolistEndpoints
    {
        public static RouteGroupBuilder MapTodolist(this IEndpointRouteBuilder routes)
        {
            var group = routes
               .MapGroup(EndpointGroupNames.TodolistsGroupName)
               .WithTags(EndpointGroupNames.TodolistsTagName)
               .RequireAuthorization();

            group.MapGet("/", async (ITodolistService todoListService, [AsParameters] TodoListQueryParam param) =>
            {
                var result = await todoListService.GetAllAsync(param);
                return Results.Ok(result);
            });

            group.MapGet("{id:int}", async (int id, ITodolistService todoListService) =>
            {
                var result = await todoListService.GetByIdAsync(id);

                if (result is null)
                    return Results.NotFound();
                else
                    return Results.Ok(result);
            });


            group.MapDelete("/{id:int}", async (int id, ITodolistService todoListService) =>
            {
                var result = await todoListService.DeleteAsync(id);

                if (result)
                    return Results.NoContent();
                else
                    return Results.NotFound(id);
            });

            group.MapPost("/", async (ITodolistService todoListService, TodolistDto dto) =>
            {
                var result = await todoListService.CreateAsync(dto);
                return Results.Ok(result);
            });

            group.MapPut("/{id:int}", async (int id, ITodolistService todoListService, TodolistDto dto) =>
            {
                var result = await todoListService.UpdateAsync(id, dto);
                if (result)
                    return Results.NoContent();
                else
                    return Results.NotFound(id);
            });

            return group;
        }
    }
}
