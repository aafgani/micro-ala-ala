using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.Web.UI.Models;
using Microsoft.AspNetCore.Authorization;
using App.Common.Domain.Auth;

namespace App.Web.UI.Controllers;

[Authorize(policy: Policy.AuthenticatedUser)]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult TestForgery(string api = "jquery")
    {
        var allowedApis = new[] { "jquery", "fetch", "axios" };

        if (!allowedApis.Contains(api.ToLower()))
        {
            throw new Exception("Invalid API.");
        }

        ViewData["Title"] = api;
        return View($"Index-{api}");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Consumes("application/x-www-form-urlencoded")]
    public IActionResult PostAjaxData(PersonViewModel personViewModel)
    {
        Task.Delay(1000).Wait(); // Simulate a delay for demonstration purposes

        // Todo: Save person to database
        return Json(new { Id = personViewModel.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    //[Consumes("application/json")]
    public IActionResult PostAjaxJson([FromBody] PersonViewModel personViewModel)
    {
        Task.Delay(1000).Wait(); // Simulate a delay for demonstration purposes

        // Todo: Save person to database
        return Json(new { Id = personViewModel.Id });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult DashboardV3()
    {
        return View();
    }
}
