using App.Api.Todo.Features.Todotask.Dtos;
using Riok.Mapperly.Abstractions;
using TodoTask = App.Api.Todo.Models.TodoTask;

namespace App.Api.Todo.Features.Todotask.Mapper
{
    [Mapper]
    public partial class TaskMapper : ITaskMapper
    {
        public TaskDto ToDto(TodoTask task)
        {
            var dto = ToDtoGenerated(task);
            dto.SubTasks = MapSubTasks(task.InverseParentTask).ToList();
            return dto;
        }

        public TodoTask ToEntity(TaskDto taskDto)
        {
            var entity = ToEntityGenerated(taskDto);
            entity.InverseParentTask = MapSubTaskEntities(taskDto.SubTasks).ToList();
            return entity;
        }

        public TodoTask ToEntity(CreateTaskDto taskDto)
        {
            var entity = ToEntityGenerated(taskDto);
            entity.InverseParentTask = MapSubTaskEntities(taskDto.SubTasks).ToList();
            foreach (var item in entity.InverseParentTask)
            {
                item.ToDoListId = entity.ToDoListId;
            }
            return entity;
        }

        [MapperIgnoreTarget(nameof(TodoTask.InverseParentTask))]
        public partial TodoTask ToEntityGenerated(CreateTaskDto taskDto);

        [MapperIgnoreTarget(nameof(TodoTask.InverseParentTask))]
        public partial TodoTask ToEntityGenerated(TaskDto taskDto);

        [MapperIgnoreTarget(nameof(TaskDto.SubTasks))]
        public partial TaskDto ToDtoGenerated(TodoTask task);

        private IEnumerable<SubtaskDto> MapSubTasks(IEnumerable<TodoTask> subTasks) =>
            subTasks?.Select(st => new SubtaskDto
            {
                Id = st.Id,
                Title = st.Title,
                Notes = st.Notes,
                IsCompleted = st.IsCompleted,
                ToDoListId = st.ToDoListId,
                DueDate = st.DueDate,
            }) ?? Enumerable.Empty<SubtaskDto>();

        private IEnumerable<TodoTask> MapSubTaskEntities(IEnumerable<SubtaskDto> subTasks) =>
            subTasks?.Select(st => new TodoTask
            {
                Id = st.Id,
                Title = st.Title,
                Notes = st.Notes,
                IsCompleted = st.IsCompleted,
                ToDoListId = st.ToDoListId
            }) ?? Enumerable.Empty<TodoTask>();

        private IEnumerable<TodoTask> MapSubTaskEntities(IEnumerable<CreateSubtaskDto> subTasks) =>
          subTasks?.Select(st => new TodoTask
          {
              Title = st.Title,
              Notes = st.Notes,
              IsCompleted = st.IsCompleted
          }) ?? Enumerable.Empty<TodoTask>();
    }
}
