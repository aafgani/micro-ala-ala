using App.Api.Todo.Extensions;
using App.Api.Todo.Features.Todos.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Api.Todo.Features.Todos.Endpoints;

public static class TodoEndpoints
{
    public static RouteGroupBuilder MapTodo(this IEndpointRouteBuilder routes)
    {
        var config = routes.ServiceProvider.GetRequiredService<IConfiguration>();
        string pathBase = config["PathBase"] ?? string.Empty;

        var group = routes
                      .MapGroup(pathBase + EndpointGroupNames.TodosGroupName)
                      .WithTags(EndpointGroupNames.TodosTagName)
                      .RequireAuthorization();

        group.MapGet("/", async (ITodoService todoService, [AsParameters] TodoListQueryParam param, HttpContext httpContext) =>
            {
                var userId = httpContext.User?.GetEmail();
                param.CreatedBy = userId;
                var result = await todoService.GetAllAsync(param);
                return (EndpointResult<PagedResult<TodolistDto>, ApiError>)result;
            });

        group.MapGet("{id:int}", async (int id, ITodoService todoService) =>
        {
            var result = await todoService.GetByIdAsync(id);
            return (EndpointResult<TodolistDto?, ApiError>)result;
        });


        group.MapDelete("/{id:int}", async (int id, ITodoService todoService) =>
        {
            var result = await todoService.DeleteAsync(id);

            return (EndpointResult<bool, ApiError>)result;
        });

        group.MapPost("/", async (ITodoService todoService, TodolistDto dto) =>
        {
            var result = await todoService.CreateAsync(dto);
            return (EndpointResult<TodolistDto, ApiError>)result;
        });

        group.MapPut("/{id:int}", async (int id, ITodoService todoService, TodolistDto dto) =>
        {
            var result = await todoService.UpdateAsync(id, dto);
            return (EndpointResult<bool, ApiError>)result;
        });

        group.MapGet("/stats", async (string? userId, ITodoService todoService) =>
        {
            var result = await todoService.GetStatsAsync(userId);
            return (EndpointResult<TodoStatsDto, ApiError>)result;
        });

        return group;
    }
}
