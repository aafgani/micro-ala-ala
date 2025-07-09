using App.Api.Todo.Models;
using Shouldly;
using Test.App.Api.Todo.IntegrationTest.Fixture;
using Test.App.Todo.Integration.Helper;

namespace Test.App.Todo.Integration.TodoTasks.RepoTests
{
    public class CreateAsyncTests : BaseIntegrationTest
    {
        public CreateAsyncTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GivenValidTodoTaskModel_CreateAsync_ShouldCreateTodoTaskAsync()
        {
            // Arrange
            var todoList = new ToDoList { Title = "Test List", UserId = "1" };
            var tags = new List<Tag> { new Tag { Name = "Urgent" } };
            var subTasks = new List<TodoTask>
            {
                new TodoTask { Title = "Sub Task 1", IsCompleted = false, CreatedAt = DateTime.UtcNow, ToDoList = todoList },
                new TodoTask { Title = "Sub Task 2", IsCompleted = false, CreatedAt = DateTime.UtcNow, ToDoList = todoList }
            };
            var todoTask = new TodoTask { 
                Title = "New Task", 
                ToDoList  = todoList,
                DueDate = DateTime.UtcNow.AddDays(7),
                IsCompleted = false,
                Tags = tags,
                InverseParentTask = subTasks
            };

            // Act
            var createdTodoTask = await TodoTaskRepository.CreateAsync(todoTask);
            await TodoContext.SaveChangesAsync();

            // Assert
            var tasksInDb = TodoContext.Tasks.Where(i => i.Id == createdTodoTask.Id).FirstOrDefault();
            tasksInDb.ShouldNotBeNull();
            tasksInDb.Title.ShouldBe(todoTask.Title);

        }
    }
}
