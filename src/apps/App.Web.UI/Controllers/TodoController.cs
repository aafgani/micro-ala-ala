using App.Common.Domain.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Controllers;

[Authorize(Policy = Policy.Todos)]
public class TodoController : Controller
{
    private readonly ILogger<TodoController> _logger;
    public TodoController(ILogger<TodoController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Navigated to Todo Index");
        return View();
    }

}
