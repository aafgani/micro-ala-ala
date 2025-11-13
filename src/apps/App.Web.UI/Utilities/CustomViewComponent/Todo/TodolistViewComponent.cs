using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using App.Web.UI.Models.Response;
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
        var result = await _todoApiClient.GetTodosAsync(1, 20);
        return (ViewComponentResultWrapper<PagedResult<TodolistDto>, ApiError>)result;
    }
}
