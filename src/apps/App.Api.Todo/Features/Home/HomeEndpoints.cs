using App.Api.Todo.Configuration;

namespace App.Api.Todo.Features.Home;

public static class Home
{
    public static WebApplication MapHomeEndpoint(this WebApplication app)
    {
        app.MapGet("/", (ApplicationInformation applicationInformation) => { return TypedResults.Text(applicationInformation.Info); })
        .AllowAnonymous();

        return app;
    }
}
