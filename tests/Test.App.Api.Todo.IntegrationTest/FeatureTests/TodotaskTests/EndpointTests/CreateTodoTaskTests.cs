using App.Api.Todo.Features.Todotask.Dtos;
using App.Api.Todo.Models;
using Shouldly;
using System.Net.Http.Json;
using Test.App.Api.Todo.IntegrationTest.Fixture;

namespace Test.App.Api.Todo.IntegrationTest.FeatureTests.TodotaskTests.EndpointTests
{
    public class CreateTodoTaskTests : BaseIntegrationTest
    {
        public CreateTodoTaskTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenCreateTodoTaskRequestWithoutSubstask_CreateTodoTask_ShouldReturnOk()
        {
            // Arrange
            var todoList = new ToDoList { Title = "Test List", UserId = "1" };
            TodoContext.ToDoLists.Add(todoList);
            await TodoContext.SaveChangesAsync();

            var createTaskDto = new CreateTaskDto
            {
                Title = "New Task",
                Notes = "This is a new task",
                DueDate = DateTime.UtcNow.AddDays(7),
                ToDoListId = todoList.Id,
                IsCompleted = false,
                SubTasks = new List<CreateSubtaskDto>
                {
                    new CreateSubtaskDto
                    {
                        Title = "New SubTask",
                        Notes = "This is a new subtask",
                        IsCompleted = false,
                        DueDate= DateTime.UtcNow.AddDays(7),
                    },
                    new CreateSubtaskDto
                    {
                        Title = "New SubTask 2",
                        Notes = "This is a new subtask 2",
                        IsCompleted = false,
                        DueDate= DateTime.UtcNow.AddDays(7),
                    }
                }
            };

            // Act
            var response = await Client.PostAsJsonAsync("/todotasks", createTaskDto);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create task: {errorContent}");
            }

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TaskDto>();
            result.ShouldNotBeNull();
            result.Title.ShouldBe(createTaskDto.Title);
            result.Notes.ShouldBe(createTaskDto.Notes);
            result.SubTasks.Count().ShouldBe(createTaskDto.SubTasks.Count());
        }


    }
}
