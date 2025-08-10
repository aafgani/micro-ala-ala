using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Controllers
{
    [AllowAnonymous]
    public class TodosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, UpdateTodoStatusRequest request)
        {
            try
            {
                // TODO: Add your actual implementation here
                // For now, return a success response for testing

                // Simulate processing
                await Task.Delay(100); // Remove this in production

                // Return JSON response expected by your JavaScript
                return Json(new
                {
                    success = true,
                    message = "Todo status updated successfully",
                    todoId = id,
                    isCompleted = request.IsCompleted
                });
            }
            catch (Exception ex)
            {
                // Log the exception (add proper logging)
                // _logger.LogError(ex, "Error updating todo status for ID: {TodoId}", id);

                return Json(new
                {
                    success = false,
                    message = "Failed to update todo status",
                    error = ex.Message // Remove in production
                });
            }
        }

    }

    public class UpdateTodoStatusRequest
    {
        public bool IsCompleted { get; set; }
    }
}
