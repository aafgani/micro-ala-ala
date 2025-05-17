using App.Api.Todo.Features.Todotask.Dtos;
using Riok.Mapperly.Abstractions;
using Task = App.Api.Todo.Models.Task;

namespace App.Api.Todo.Features.Todotask.Mapper
{
    [Mapper]
    public partial class TaskMapper : ITaskMapper
    {
        public TaskDto ToDto(Task task)
        {
            var dto = ToDtoGenerated(task);
            dto.SubTasks = MapSubTasks(task.InverseParentTask).ToList();
            return dto;
        }

        public Task ToEntity(TaskDto taskDto)
        {
            var entity = ToEntityGenerated(taskDto);
            entity.InverseParentTask = MapSubTaskEntities(taskDto.SubTasks).ToList();
            return entity;
        }

        public Task ToEntity(CreateTaskDto taskDto)
        {
            var entity = ToEntityGenerated(taskDto);
            entity.InverseParentTask = MapSubTaskEntities(taskDto.SubTasks).ToList();
            foreach (var item in entity.InverseParentTask)
            {
                item.ToDoListId = entity.ToDoListId; 
            }
            return entity;
        }

        [MapperIgnoreTarget(nameof(Task.InverseParentTask))]
        public partial Task ToEntityGenerated(CreateTaskDto taskDto);

        [MapperIgnoreTarget(nameof(Task.InverseParentTask))]
        public partial Task ToEntityGenerated(TaskDto taskDto);

        [MapperIgnoreTarget(nameof(TaskDto.SubTasks))]
        public partial TaskDto ToDtoGenerated(Task task);

        private IEnumerable<SubtaskDto> MapSubTasks(IEnumerable<Task> subTasks) =>
            subTasks?.Select(st => new SubtaskDto
            {
                Id = st.Id,
                Title = st.Title,
                Notes = st.Notes,
                IsCompleted = st.IsCompleted
            }) ?? Enumerable.Empty<SubtaskDto>();

        private IEnumerable<Task> MapSubTaskEntities(IEnumerable<SubtaskDto> subTasks) =>
            subTasks?.Select(st => new Task
            {
                Id = st.Id,
                Title = st.Title,
                Notes = st.Notes,
                IsCompleted = st.IsCompleted,
                ToDoListId = st.ToDoListId
            }) ?? Enumerable.Empty<Task>();

        private IEnumerable<Task> MapSubTaskEntities(IEnumerable<CreateSubtaskDto> subTasks) =>
          subTasks?.Select(st => new Task
          {
              Title = st.Title,
              Notes = st.Notes,
              IsCompleted = st.IsCompleted
          }) ?? Enumerable.Empty<Task>();
    }
}
