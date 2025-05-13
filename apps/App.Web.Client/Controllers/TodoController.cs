using App.Common.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Controllers
{
    public class TodoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }

        public IActionResult GetDetail(int taskId)
        {
            /*var task = _taskService.GetById(taskId); */// Fetch the task details
            var task = new TodoDto(
                Id: taskId,
                Title: "Sample Task",
                Description: "This is a sample task description.",
                DueDate: DateTime.Now.AddDays(7),
                IsCompleted: false,
                Tags: new[] { "Sample", "Task" },
                AssignedTo: "John Doe",
                CreatedAt: DateTime.Now,
                UpdatedAt: DateTime.Now
            );
            return PartialView("_Detail", task);     // Return partial view with model
        }
    }
}
