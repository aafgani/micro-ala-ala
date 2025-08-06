using System.Net;
using App.Api.Todo.Features.Todos.Data;
using App.Api.Todo.Features.Todos.Mapper;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Api.Todo.Features.Todos.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly ITodoMapper _mapper;

    public TodoService(ITodoRepository todoRepository, ITodoMapper mapper)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
    }

    public async Task<Result<TodolistDto, ApiError>> CreateAsync(TodolistDto dto)
    {
        var todo = _mapper.ToEntity(dto);
        var result = await _todoRepository.CreateAsync(todo);
        return _mapper.ToDto(result);
    }

    public async Task<Result<bool, ApiError>> DeleteAsync(int id)
    {
        var todo = await _todoRepository.GetByIdAsync(id);
        if (todo is null)
            return false;

        await _todoRepository.DeleteAsync(todo);
        return true;
    }

    public async Task<Result<PagedResult<TodolistDto>, ApiError>> GetAllAsync(TodoListQueryParam queryParam)
    {
        try
        {
            if (queryParam == null)
                throw new ArgumentNullException(nameof(queryParam));

            queryParam.ApplyDefaults();

            var (todos, totalItems, totalPages) = await _todoRepository.GetWithParamAsync(queryParam);

            var dtoList = todos?.Select(_mapper.ToDto).ToList() ?? new List<TodolistDto>();

            return new PagedResult<TodolistDto>
            {
                Data = dtoList,
                Pagination = new PaginationMetadata
                {
                    CurrentPage = queryParam.Page,
                    PageSize = queryParam.PageSize,
                    TotalItems = totalItems,

                }
            };
        }
        catch (Exception ex)
        {
            return new ApiError($"Failed to retrieve todolists: {ex.Message}", HttpStatusCode.InternalServerError); // Uses implicit operator
        }
    }

    public async Task<Result<TodolistDto?, ApiError>> GetByIdAsync(int id)
    {
        var todo = await _todoRepository.GetByIdAsync(id);
        if (todo is null)
            return new ApiError("Todolist not found", HttpStatusCode.NotFound);

        return _mapper.ToDto(todo);
    }

    public async Task<Result<bool, ApiError>> UpdateAsync(int id, TodolistDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var existingTodo = await _todoRepository.GetByIdAsync(id);
        if (existingTodo is null)
            return new ApiError("Todolist not found", HttpStatusCode.NotFound);

        existingTodo.Title = dto.Title;
        existingTodo.DueDate = dto.DueDate;
        existingTodo.AssignTo = dto.UserId;
        existingTodo.IsCompleted = dto.IsCompleted;
        existingTodo.Notes = dto.Description;

        try
        {
            await _todoRepository.UpdateAsync(existingTodo);
            return true;
        }
        catch (Exception ex)
        {
            return new ApiError($"Failed to update todolist: {ex.Message}", HttpStatusCode.InternalServerError); // Uses implicit operator
        }
    }
}
