using App.Web.Client.Utilities.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Utilities.CustomViewComponent.Todos;

public class TodolistStatsViewComponent : ViewComponent
{
    private readonly ITodoApiClient _todoApiClient;

    public TodolistStatsViewComponent(ITodoApiClient todoApiClient)
    {
        _todoApiClient = todoApiClient;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var stats = await _todoApiClient.GetTodoStatsAsync();
        return View(stats);
    }
}
