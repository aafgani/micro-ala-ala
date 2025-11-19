using App.Api.Todo.Features.Todotask.Dtos;
using App.Api.Todo.Features.Todotask.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Api.Todo.Features.Todotask.Endpoints
{
    public static class TodoTaksEndpoints
    {
        public static RouteGroupBuilder MapTodoTasks(this IEndpointRouteBuilder routes)
        {
            var config = routes.ServiceProvider.GetRequiredService<IConfiguration>();
            string pathBase = config["PathBase"] ?? string.Empty;

            var group = routes
              .MapGroup(pathBase + EndpointGroupNames.TodotasksGroupName)
              .WithTags(EndpointGroupNames.TodotasksTagName)
              .RequireAuthorization();

            group.MapGet("/", async (ITodoTaskService todoTaskService, [AsParameters] TodoTaskQueryParam param) =>
            {
                var result = await todoTaskService.GetAllAsync(param);
                return (EndpointResult<PagedResult<TaskDto>, ApiError>)result;
            });

            group.MapGet("{id:int}", async (int id, ITodoTaskService todoTaskService) =>
            {
                var result = await todoTaskService.GetByIdAsync(id);

                return (EndpointResult<TaskDto?, ApiError>)result;
            });

            group.MapDelete("/{id:int}", async (int id, ITodoTaskService todoTaskService) =>
            {
                var result = await todoTaskService.DeleteAsync(id);

                return (EndpointResult<bool, ApiError>)result;
            });

            group.MapPost("/", async (ITodoTaskService todoTaskService, CreateTaskDto dto) =>
            {
                var result = await todoTaskService.CreateAsync(dto);
                return (EndpointResult<TaskDto, ApiError>)result;
            });

            group.MapPut("/{id:int}", async (int id, ITodoTaskService todoTaskService, UpdateTaskDto dto) =>
            {
                var result = await todoTaskService.UpdateAsync(dto.Id, dto);
                return (EndpointResult<bool, ApiError>)result;
            });

            return group;
        }
    }
}
