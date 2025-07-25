﻿using System.Net;
using App.Api.Todo.Features.Todolist.Data;
using App.Api.Todo.Features.Todolist.Mapper;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Api.Todo.Features.Todolist.Services
{
    public class TodolistService : ITodolistService
    {
        private readonly ITodolistRepository _todolistRepository;
        private readonly ITodoListMapper _mapper;

        public TodolistService(ITodolistRepository todolistRepository, ITodoListMapper todolistMapper)
        {
            _todolistRepository = todolistRepository;
            _mapper = todolistMapper;
        }

        public async Task<Result<TodolistDto, ApiError>> CreateAsync(TodolistDto dto)
        {
            var todolist = _mapper.ToEntity(dto);
            var result = await _todolistRepository.CreateAsync(todolist);
            return _mapper.ToDto(result);
        }

        public async Task<Result<bool, ApiError>> DeleteAsync(int id)
        {
            var todolist = await _todolistRepository.GetByIdAsync(id);
            if (todolist is null)
                return false;

            try
            {
                await _todolistRepository.DeleteAsync(todolist);
                return true;
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                // _logger?.LogError(ex, "Failed to delete ToDoList with id {Id}", id);
                return false;
            }
        }

        public async Task<Result<PagedResult<TodolistDto>, ApiError>> GetAllAsync(TodoListQueryParam queryParam)
        {
            try
            {
                if (queryParam == null)
                    throw new ArgumentNullException(nameof(queryParam));

                queryParam.ApplyDefaults();

                var (todolists, totalItems, totalPages) = await _todolistRepository.GetWithParamAsync(queryParam);

                var dtoList = todolists?.Select(_mapper.ToDto).ToList() ?? new List<TodolistDto>();

                return new PagedResult<TodolistDto>
                {
                    Data = dtoList,
                    Pagination = new PaginationMetadata
                    {
                        CurrentPage = queryParam.Page,
                        PageSize = queryParam.PageSize,
                        TotalItems = totalItems,
                        TotalPages = totalPages
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
            var task = await _todolistRepository.GetByIdAsync(id);
            if (task is null)
                return new ApiError("Todolist not found", HttpStatusCode.NotFound);

            return _mapper.ToDto(task);
        }

        public async Task<Result<bool, ApiError>> UpdateAsync(int id, TodolistDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existingTodoList = await _todolistRepository.GetByIdAsync(id);
            if (existingTodoList is null)
                return false;

            existingTodoList.Title = dto.Title;
            existingTodoList.UserId = dto.UserId;
            existingTodoList.Description = dto.Description;

            try
            {
                await _todolistRepository.UpdateAsync(existingTodoList);
                return true;
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                // _logger?.LogError(ex, "Failed to update ToDoList with id {Id}", id);
                return false;
            }
        }
    }
}
