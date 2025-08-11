using System;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Utilities.CustomViewComponent;

public class TodolistViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        // Here you can fetch data from a database or any other source
        // For demonstration, we will return a simple list of todos
        var todos = new List<string>
        {
            "Buy groceries",
            "Walk the dog",
            "Read a book"
        };

        Task.Delay(2000).Wait(); // Simulate a delay for loading

        return View(todos);
    }
}
