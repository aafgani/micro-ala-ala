using App.Api.Todo.Features.Todotask.Data;
using App.Api.Todo.Features.Todotask.Dtos;
using App.Api.Todo.Features.Todotask.Mapper;
using App.Common.Domain.Pagination;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Todo.Features.Todotask.Services
{
    public class TodoTaskService : ITodoTaskService
    {
        private readonly ITodoTaskRepository _todoTaskRepository;
        private readonly ITaskMapper _taskMapper;

        public TodoTaskService(ITodoTaskRepository todoTaskRepository, ITaskMapper taskMapper)
        {
            _todoTaskRepository = todoTaskRepository;
            _taskMapper = taskMapper;
        }

        public async Task<TaskDto> CreateAsync(CreateTaskDto dto)
        {
            var task = _taskMapper.ToEntity(dto);
            var result = await _todoTaskRepository.CreateAsync(task);
            return _taskMapper.ToDto(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _todoTaskRepository.GetByIdAsync(id); 
            if (task is null) return false; 

            await _todoTaskRepository.DeleteAsync(task);
            return true; 
        }

        public async Task<PagedResult<TaskDto>> GetAllAsync(TodoTaskQueryParam queryParam)
        {
            queryParam.ApplyDefaults();

            var (tasks, totalItems, totalPages) = await _todoTaskRepository.GetWithParamAsync(queryParam);

            var test = tasks.Select(task => _taskMapper.ToDto(task));

            return new PagedResult<TaskDto>
            {
                Data = tasks.Select(_taskMapper.ToDto),
                Pagination = new PaginationMetadata
                {
                    CurrentPage = queryParam.Page,
                    PageSize = queryParam.PageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }
       
        public async Task<TaskDto?> GetByIdAsync(int id)
        {
           return await Task.Run(() =>
            {
                var task = _todoTaskRepository.Query()
                   .Include(task => task.InverseParentTask)
                   .Include(task => task.Tags)
                   .Where(task => task.Id == id)
                   .FirstOrDefault();

                return task is null ? null : _taskMapper.ToDto(task);
            });
        }
        
        public Task<bool> UpdateAsync(int id, TaskDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
