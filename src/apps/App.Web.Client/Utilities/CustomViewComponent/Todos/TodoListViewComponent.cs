using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Utilities.CustomViewComponent.Todos;

public class TodolistViewComponent : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiBaseUrl;

    public TodolistViewComponent(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _apiBaseUrl = configuration["TodoApiBaseUrl"] ?? "http://localhost:8081";
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var client = _httpClientFactory.CreateClient();
        var pagedResult = await client.GetFromJsonAsync<PagedResult<TodolistDto>>(_apiBaseUrl + "/todos?Page=1&PageSize=5");

        return View(pagedResult);
    }
}
