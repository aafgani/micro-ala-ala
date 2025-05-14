using App.Api.Todo.Features.Todolist.Endpoints;

namespace App.Api.Todo.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapTodolist();

            return app;
        }
    }
}
