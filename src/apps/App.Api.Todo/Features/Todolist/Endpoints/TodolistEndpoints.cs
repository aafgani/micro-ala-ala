using App.Api.Todo.Features.Todolist.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

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
                return (EndpointResult<PagedResult<TodolistDto>, ApiError>)result;
            });

            group.MapGet("{id:int}", async (int id, ITodolistService todoListService) =>
            {
                var result = await todoListService.GetByIdAsync(id);

                return (EndpointResult<TodolistDto?, ApiError>)result;
            });


            group.MapDelete("/{id:int}", async (int id, ITodolistService todoListService) =>
            {
                var result = await todoListService.DeleteAsync(id);

                return (EndpointResult<bool, ApiError>)result;
            });

            group.MapPost("/", async (ITodolistService todoListService, TodolistDto dto) =>
            {
                var result = await todoListService.CreateAsync(dto);
                return (EndpointResult<TodolistDto, ApiError>)result;
            });

            group.MapPut("/{id:int}", async (int id, ITodolistService todoListService, TodolistDto dto) =>
            {
                var result = await todoListService.UpdateAsync(id, dto);
                return (EndpointResult<bool, ApiError>)result;
            });

            return group;
        }
    }
}
