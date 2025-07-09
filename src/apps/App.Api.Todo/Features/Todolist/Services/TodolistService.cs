using App.Api.Todo.Features.Todolist.Data;
using App.Api.Todo.Features.Todolist.Mapper;
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

        public async Task<TodolistDto> CreateAsync(TodolistDto dto)
        {
            var todolist = _mapper.ToEntity(dto);
            var result = await _todolistRepository.CreateAsync(todolist);
            return _mapper.ToDto(result);
        }

        public async Task<bool> DeleteAsync(int id)
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

        public async Task<PagedResult<TodolistDto>> GetAllAsync(TodoListQueryParam queryParam)
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

        public async Task<TodolistDto?> GetByIdAsync(int id)
        {
            var task = await _todolistRepository.GetByIdAsync(id);
            return task is null ? null : _mapper.ToDto(task);
        }

        public Task<bool> UpdateAsync(int id, TodolistDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return Task.Run(async () =>
            {
                var existingTodoList = await _todolistRepository.GetByIdAsync(id);
                if (existingTodoList is null)
                    return false;

                var updatedTodoList = _mapper.ToEntity(dto);
                updatedTodoList.Id = id; // Ensure the ID is set to the existing one

                try
                {
                    await _todolistRepository.UpdateAsync(updatedTodoList);
                    return true;
                }
                catch (Exception ex)
                {
                    // Optionally log the exception here
                    // _logger?.LogError(ex, "Failed to update ToDoList with id {Id}", id);
                    return false;
                }
            });
        }
    }
}
