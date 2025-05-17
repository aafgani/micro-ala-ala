using App.Api.Todo.Features.Tags.Endpoints;
using App.Api.Todo.Features.Todotask.Endpoints;

namespace App.Api.Todo.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapTags();
            app.MapTodoTasks();

            return app;
        }
    }
}
