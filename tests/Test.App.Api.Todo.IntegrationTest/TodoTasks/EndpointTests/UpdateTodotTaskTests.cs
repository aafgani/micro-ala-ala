using System.Net.Http.Json;
using App.Api.Todo.Features.Todotask.Dtos;
using App.Api.Todo.Models;
using App.Common.Domain.Dtos.Todo;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoTasks.EndpointTests;

public class UpdateTodotTaskTests : BaseIntegrationTest
{
    public UpdateTodotTaskTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GivenUpdateTodoTask__UpdateTodoTask_ShouldReturnOk()
    {
        // Arrange
        var todoList = new ToDoList { Title = "Test List", UserId = "1" };

        var todoTask = new TodoTask
        {
            Title = "New Task",
            Notes = "This is a new task",
            DueDate = DateTime.UtcNow.AddDays(7),
            ToDoList = todoList,
            IsCompleted = false,
            InverseParentTask = new List<TodoTask>
                {
                    new TodoTask
                    {
                        Title = "New SubTask",
                        Notes = "This is a new subtask",
                        IsCompleted = false,
                        DueDate= DateTime.UtcNow.AddDays(7),
                        ToDoList = todoList
                    },
                    new TodoTask
                    {
                        Title = "New SubTask 2",
                        Notes = "This is a new subtask 2",
                        IsCompleted = false,
                        DueDate= DateTime.UtcNow.AddDays(7),
                        ToDoList = todoList
                    }
                }
        };
        TodoContext.ToDoLists.Add(todoList);
        TodoContext.Tasks.Add(todoTask);

        await TodoContext.SaveChangesAsync();

        var updateTaskDto = new UpdateTaskDto
        {
            Id = todoTask.Id,
            Title = "Updated Task",
            Notes = "Updated notes",
            DueDate = DateTime.UtcNow.AddDays(7),
            ToDoListId = todoList.Id,
            IsCompleted = false,
            SubTasks = new List<SubtaskDto>
                {
                    new SubtaskDto
                    {
                        Id = todoTask.InverseParentTask.First().Id,
                        Title = "Updated SubTask",
                        Notes = "This is a updated subtask",
                        IsCompleted = false,
                        DueDate= DateTime.UtcNow.AddDays(7),
                        ToDoListId = todoList.Id
                    },
                    new SubtaskDto
                    {
                        Id = todoTask.InverseParentTask.Last().Id,
                        Title = "Updated SubTask 2",
                        Notes = "This is a updated subtask 2",
                        IsCompleted = false,
                        DueDate= DateTime.UtcNow.AddDays(7),
                        ToDoListId = todoList.Id
                    }
                }
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/tasks/{todoTask.Id}", updateTaskDto);
        if (!response.StatusCode.Equals(System.Net.HttpStatusCode.NoContent))
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to update task: {errorContent}");
        }

        // Assert

    }
}
