using App.Api.Todo.Models;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoTasks.RepoTests
{
    public class UpdateAsyncTests : BaseIntegrationTest
    {
        public UpdateAsyncTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidTodoTaskModel_UpdateAsync_ShouldUpdateTodoTaskAsync()
        {
            // Arrange: create and add a new TodoTask
            var todoList = new ToDoList { Title = "Test List", UserId = "1" };
            var subTasks = new List<TodoTask>
            {
                new TodoTask { Title = "Sub Task 1", IsCompleted = false, CreatedAt = DateTime.UtcNow, ToDoList = todoList },
                new TodoTask { Title = "Sub Task 2", IsCompleted = false, CreatedAt = DateTime.UtcNow, ToDoList = todoList }
            };
            var todoTask = new TodoTask
            {
                Title = "Initial Title",
                Notes = "Initial Description",
                IsCompleted = false,
                ToDoList = todoList,
                InverseParentTask = subTasks,
            };
            await TodoTaskRepository.CreateAsync(todoTask);

            // Act: update the task
            todoTask.Title = "Updated Title";
            todoTask.IsCompleted = true;
            todoTask.InverseParentTask.First().Title = "Updated Sub Task 1"; // Update a sub-task as well
            todoTask.InverseParentTask.Remove(todoTask.InverseParentTask.LastOrDefault()); // Remove the second sub-task
            await TodoTaskRepository.UpdateAsync(todoTask);

            // Assert: fetch from context and verify update
            var updatedTask = await TodoContext.Tasks.FindAsync(todoTask.Id);
            todoTask.InverseParentTask.ShouldNotBeNull();
            updatedTask.Title.ShouldBe("Updated Title"); // Title should be updated
            updatedTask.Notes.ShouldBe("Initial Description"); // Notes should remain unchanged
            updatedTask.IsCompleted.ShouldBeTrue(); // IsCompleted should be true
            updatedTask.InverseParentTask.Count.ShouldBe(1); // One sub-task should remain
            updatedTask.InverseParentTask.First().Title.ShouldBe("Updated Sub Task 1"); // The remaining sub-task should be updated
        }
    }
}
