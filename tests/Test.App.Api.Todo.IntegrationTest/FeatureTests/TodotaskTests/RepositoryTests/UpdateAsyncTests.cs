using App.Api.Todo.Models;
using Test.App.Api.Todo.IntegrationTest.Fixture;

namespace Test.App.Api.Todo.IntegrationTest.FeatureTests.TodotaskTests.RepositoryTests
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
            await TodoTaskRepository.UpdateAsync(todoTask);

            // Assert: fetch from context and verify update
            var updatedTask = await TodoContext.Tasks.FindAsync(todoTask.Id);
            Assert.NotNull(updatedTask);
            Assert.Equal("Updated Title", updatedTask.Title);
            Assert.True(updatedTask.IsCompleted);
        }
    }
}
