using App.Web.Client.Utilities.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Utilities.CustomViewComponent.Todos;

public class TodolistViewComponent : ViewComponent
{
    private readonly ITodoApiClient _todoApiClient;

    public TodolistViewComponent(ITodoApiClient todoApiClient)
    {
        _todoApiClient = todoApiClient;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var pagedResult = await _todoApiClient.GetTodosAsync(1, 5);
        return View(pagedResult);
    }
}
