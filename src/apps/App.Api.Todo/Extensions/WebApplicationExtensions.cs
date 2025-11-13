using App.Api.Todo.Features.Home;
using App.Api.Todo.Features.Tags.Endpoints;
using App.Api.Todo.Features.Todos.Endpoints;
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
            app.MapTodo();
            return app;
        }

        public static WebApplication ConfigureRouting(this WebApplication app, IConfiguration config)
        {
            var pathBase = config.GetValue<string>("PathBase");
            if (!string.IsNullOrEmpty(pathBase))
                app.UsePathBase(pathBase);

            app.UseRouting();

            return app;
        }
    }
}
