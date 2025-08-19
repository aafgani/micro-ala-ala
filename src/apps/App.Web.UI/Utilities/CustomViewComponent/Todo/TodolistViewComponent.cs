using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Utilities.CustomViewComponent.Todo;

public class TodolistViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Here you can fetch data from a database or any other source
        // For demonstration, we will return a simple list of todos
        var todos = new List<string>
        {
            "Buy groceries",
            "Walk the dog",
            "Read a book"
        };

        await Task.Delay(500); // Simulate a delay for loading

        return View(todos);
    }
}
