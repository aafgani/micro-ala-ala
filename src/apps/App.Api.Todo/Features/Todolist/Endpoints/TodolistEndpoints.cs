using System.Net;
using App.Api.Todo.Dtos.Validators;
using App.Api.Todo.Features.Todolist.Services;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using FluentValidation;

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
                if (id <= 0)
                {
                    return Results.BadRequest(new ApiError("Id must be greater than zero.", HttpStatusCode.BadRequest));
                }

                var result = await todoListService.DeleteAsync(id);

                return (EndpointResult<bool, ApiError>)result;
            });

            group.MapPost("/", async (ITodolistService todoListService, IValidator<TodolistDto> validator, TodolistDto dto) =>
            {
                var validationResult = await dto.ValidateAsync(validator);
                if (!validationResult.IsSuccess)
                {
                    return (EndpointResult<TodolistDto, ApiError>)validationResult;
                }
                if (dto.Id != 0)
                {
                    return Results.BadRequest(new ApiError("d should not be set for creation.", HttpStatusCode.BadRequest));
                }

                var result = await todoListService.CreateAsync(dto);
                return (EndpointResult<TodolistDto, ApiError>)result;
            });

            group.MapPut("/{id:int}", async (int id, ITodolistService todoListService, IValidator<TodolistDto> validator, TodolistDto dto) =>
            {
                var validationResult = await dto.ValidateAsync(validator);
                if (!validationResult.IsSuccess)
                {
                    return (EndpointResult<TodolistDto, ApiError>)validationResult;
                }
                if (id <= 0)
                {
                    return Results.BadRequest(new ApiError("Id must be greater than zero.", HttpStatusCode.BadRequest));
                }

                var result = await todoListService.UpdateAsync(id, dto);
                return (EndpointResult<bool, ApiError>)result;
            });

            return group;
        }
    }
}
