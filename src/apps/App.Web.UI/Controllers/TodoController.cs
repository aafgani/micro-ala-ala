using App.Common.Domain.Auth;
using App.Common.Domain.Dtos.ApiResponse;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using App.Web.UI.Extensions;
using App.Web.UI.Models.Dto;
using App.Web.UI.Models.Response;
using App.Web.UI.Utilities.Http.Todo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Controllers;

[Authorize(Policy = Policy.Todos)]
public class TodoController : Controller
{
    private readonly ITodoApiClient _todoApiClient;
    private readonly ILogger<TodoController> _logger;
    public TodoController(ITodoApiClient todoApiClient, ILogger<TodoController> logger)
    {
        _todoApiClient = todoApiClient;
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Navigated to Todo Index");
        return View();
    }

    #region ActionResult
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTodoAsync([FromBody] AddTodo model)
    {
        _logger.LogInformation("Adding Todo with Title: {Title} and DueDate: {DueDate}", model.Title, model.DueDate);

        if (string.IsNullOrWhiteSpace(model.Title))
        {
            ModelState.AddModelError("Title", "Title is required.");
        }

        if (model.DueDate == default)
        {
            ModelState.AddModelError("DueDate", "Due date is required.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is invalid. Errors: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return BadRequest(ModelState);
        }

        var todolistDto = new TodolistDto
        {
            Title = model.Title,
            DueDate = model.DueDate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User.GetEmail(),
            UserId = User.GetEmail()
        };

        var result = await _todoApiClient.CreateTodoAsync(todolistDto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTodosAsync([FromBody] AddTodo model)
    {
        var pagedResult = await _todoApiClient.GetTodosAsync(1, 20);
        return (MvcEndpointResult<PagedResult<TodolistDto>, ApiError>)pagedResult;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTodo(int id, [FromBody] TodoDto dto)
    {
        var result = await _todoApiClient.UpdateTodoAsync(id, dto);
        return (MvcEndpointResult<bool, ApiError>)result;
    }

    [HttpDelete]
    public IActionResult DeleteTodo(int id)
    {
        // _todoApiClient.Delete(id);
        // return Ok(new { success = true });
        return Ok(new { success = true });
    }
    #endregion

    #region PartialView
    public async Task<IActionResult> GetTodoPartialViewAsync()
    {
        var result = await _todoApiClient.GetTodosAsync(1, 20);
        return PartialView("Partial/_TodoPartialView", result.Value);
    }
    #endregion

}
