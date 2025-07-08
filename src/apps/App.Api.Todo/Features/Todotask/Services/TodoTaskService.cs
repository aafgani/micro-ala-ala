using App.Api.Todo.Features.Todotask.Data;
using App.Api.Todo.Features.Todotask.Dtos;
using App.Api.Todo.Features.Todotask.Mapper;
using App.Common.Domain.Dtos;
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

        public async Task<bool> UpdateAsync(int id, TaskDto dto)
        {
            // Verify the task exists before attempting to update
            var existingTask = await _todoTaskRepository.GetByIdWithRelationsAsync(id);
            if (existingTask is null) return false;

            // Ensure the DTO ID matches the requested ID
            if (dto.Id != id)
            {
                dto.Id = id; // Force ID to match the requested ID
            }

            try
            {
                // Update the properties of the existing entity instead of creating a new one
                existingTask.Title = dto.Title;
                existingTask.Notes = dto.Notes;
                existingTask.IsCompleted = dto.IsCompleted;
                existingTask.DueDate = dto.DueDate;
                existingTask.ToDoListId = dto.ToDoListId;

                // Handle subtasks carefully to avoid tracking conflicts
                if (dto.SubTasks != null)
                {
                    // Create a dictionary of existing subtasks by ID for easy lookup
                    var existingSubtasks = existingTask.InverseParentTask.ToDictionary(st => st.Id);

                    // Create a new list to hold the updated subtasks
                    var updatedSubtasks = new List<Models.TodoTask>();

                    foreach (var subtaskDto in dto.SubTasks)
                    {
                        if (subtaskDto.Id > 0 && existingSubtasks.TryGetValue(subtaskDto.Id, out var existingSubtask))
                        {
                            // Update existing subtask
                            existingSubtask.Title = subtaskDto.Title;
                            existingSubtask.Notes = subtaskDto.Notes;
                            existingSubtask.IsCompleted = subtaskDto.IsCompleted;
                            existingSubtask.ToDoListId = subtaskDto.ToDoListId;

                            updatedSubtasks.Add(existingSubtask);
                        }
                        else
                        {
                            // Add new subtask
                            var newSubtask = new Models.TodoTask
                            {
                                Title = subtaskDto.Title,
                                Notes = subtaskDto.Notes,
                                IsCompleted = subtaskDto.IsCompleted,
                                ToDoListId = subtaskDto.ToDoListId,
                                ParentTaskId = id
                            };

                            updatedSubtasks.Add(newSubtask);
                        }
                    }

                    existingTask.InverseParentTask.Clear();
                    foreach (var subtask in updatedSubtasks)
                    {
                        existingTask.InverseParentTask.Add(subtask);
                    }
                }

                // Handle tags collection - we'll just replace it with the tags from the DTO
                // This is a simpler approach since tags are typically just references
                // if (dto.Tags != null)
                // {
                //     // Load the task with its tags
                //     var taskWithTags = await _todoTaskRepository.Query(true)
                //         .Include(t => t.Tags)
                //         .FirstOrDefaultAsync(t => t.Id == id);

                //     if (taskWithTags != null)
                //     {
                //         // Clear existing tags
                //         taskWithTags.Tags.Clear();

                //         // Add tags from DTO
                //         foreach (var tagDto in dto.Tags)
                //         {
                //             // Here we assume that Tag entities already exist in the database
                //             // and we're just creating references to them
                //             var tag = new Models.Tag { Id = tagDto.Id };
                //             _db.Attach(tag); // Attach the tag to the context without loading it
                //             taskWithTags.Tags.Add(tag);
                //         }
                //     }
                // }

                var result = await _todoTaskRepository.UpdateAsync(existingTask);
                return result > 0; // Return true if the update was successful, false otherwise
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating task: {ex.Message}");
                return false;
            }
        }
    }
}
