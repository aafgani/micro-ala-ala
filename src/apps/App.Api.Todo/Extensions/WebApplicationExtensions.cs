using App.Api.Todo.Features.Home;
using App.Api.Todo.Features.Tags.Endpoints;
using App.Api.Todo.Features.Todolist.Endpoints;
using App.Api.Todo.Features.Todotask.Endpoints;

namespace App.Api.Todo.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapHomeEndpoint();
            app.MapTags();
            app.MapTodoTasks();
            app.MapTodolist();

            return app;
        }
    }
}
