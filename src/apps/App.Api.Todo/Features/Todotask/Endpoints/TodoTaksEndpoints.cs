using App.Api.Todo.Features.Todotask.Dtos;
using App.Api.Todo.Features.Todotask.Services;
using App.Common.Domain.Dtos;

namespace App.Api.Todo.Features.Todotask.Endpoints
{
    public static class TodoTaksEndpoints
    {
        public static RouteGroupBuilder MapTodoTasks(this IEndpointRouteBuilder routes)
        {
            var group = routes
              .MapGroup(EndpointGroupNames.TodotasksGroupName)
              .WithTags(EndpointGroupNames.TodotasksTagName);

            group.MapGet("/", async (ITodoTaskService todoTaskService, [AsParameters] TodoTaskQueryParam param) =>
            {
                var result = await todoTaskService.GetAllAsync(param);
                return Results.Ok(result);
            });

            group.MapGet("{id:int}", async (int id, ITodoTaskService todoTaskService) =>
            {
                var result = await todoTaskService.GetByIdAsync(id);

                if (result is null)
                    return Results.NotFound();
                else
                    return Results.Ok(result); ;
            });

            group.MapDelete("/{id:int}", async (int id, ITodoTaskService todoTaskService) =>
            {
                var result = await todoTaskService.DeleteAsync(id);

                if (result)
                    return Results.NoContent();
                else
                    return Results.NotFound(id);
            });

            group.MapPost("/", async (ITodoTaskService todoTaskService, CreateTaskDto dto) =>
            {
                var result = await todoTaskService.CreateAsync(dto);
                return Results.Ok(result);
            });

            group.MapPut("/{id:int}", async (int id, ITodoTaskService todoTaskService, UpdateTaskDto dto) =>
            {
                var result = await todoTaskService.UpdateAsync(dto.Id, dto);
                if (result)
                    return Results.NoContent();
                else
                    return Results.NotFound(dto.Id);
            });

            return group;
        }
    }
}
