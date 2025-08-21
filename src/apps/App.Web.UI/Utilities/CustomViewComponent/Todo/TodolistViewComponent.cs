using App.Web.UI.Utilities.Http;
using App.Web.UI.Utilities.Http.Todo;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Utilities.CustomViewComponent.Todo;

public class TodolistViewComponent : ViewComponent
{
    private readonly ITodoApiClient _todoApiClient;

    public TodolistViewComponent(ITodoApiClient todoApiClient)
    {
        _todoApiClient = todoApiClient;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var pagedResult = await _todoApiClient.GetTodosAsync(1, 20);
        return View(pagedResult);
    }
}
