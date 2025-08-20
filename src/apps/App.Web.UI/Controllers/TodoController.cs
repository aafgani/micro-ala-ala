using App.Common.Domain.Auth;
using App.Common.Domain.Dtos.Todo;
using App.Web.UI.Extensions;
using App.Web.UI.Models.Dto;
using App.Web.UI.Utilities.Http;
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTodo([FromBody] AddTodo model)
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

        var result = _todoApiClient.CreateTodoAsync(todolistDto);
        return Json(result);
    }
}
