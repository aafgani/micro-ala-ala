using App.Common.Domain.Dtos.TodoApi;
using App.Common.Domain.Pagination;
using App.Common.Infrastructure.HttpHandler;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Controllers
{
    public class TodoController : Controller
    {
        private readonly IApiClient _todoApiClient;

        // Dummy data for tasks
        private static List<TodoDto> _tasks = new List<TodoDto>
        {
            new TodoDto(
                Id: 1,
                Title: "Complete project documentation",
                Description: "Write comprehensive documentation for the API endpoints",
                DueDate: DateTime.Now.AddDays(5),
                IsCompleted: false,
                Tags: new[] { "Work", "Documentation" },
                AssignedTo: "John Doe",
                CreatedAt: DateTime.Now.AddDays(-2),
                UpdatedAt: DateTime.Now.AddDays(-1)
            ),
            new TodoDto(
                Id: 2,
                Title: "Fix UI bugs in dashboard",
                Description: "Address the reported UI issues in the analytics dashboard",
                DueDate: DateTime.Now.AddDays(3),
                IsCompleted: true,
                Tags: new[] { "Bug", "UI", "Important" },
                AssignedTo: "Jane Smith",
                CreatedAt: DateTime.Now.AddDays(-5),
                UpdatedAt: DateTime.Now.AddDays(-1)
            ),
            new TodoDto(
                Id: 3,
                Title: "Prepare for client meeting",
                Description: "Create presentation slides and demo for the upcoming client meeting",
                DueDate: DateTime.Now.AddDays(1),
                IsCompleted: false,
                Tags: new[] { "Meeting", "Client", "Important" },
                AssignedTo: "Mike Johnson",
                CreatedAt: DateTime.Now.AddDays(-3),
                UpdatedAt: DateTime.Now.AddDays(-2)
            ),
            new TodoDto(
                Id: 4,
                Title: "Review pull requests",
                Description: "Review and approve pending pull requests for the feature branch",
                DueDate: DateTime.Now.AddDays(2),
                IsCompleted: false,
                Tags: new[] { "Code", "Review" },
                AssignedTo: "John Doe",
                CreatedAt: DateTime.Now.AddDays(-1),
                UpdatedAt: DateTime.Now.AddDays(-1)
            ),
            new TodoDto(
                Id: 5,
                Title: "Update dependencies",
                Description: "Update project dependencies to latest stable versions",
                DueDate: DateTime.Now.AddDays(7),
                IsCompleted: true,
                Tags: new[] { "Maintenance", "Code" },
                AssignedTo: "Jane Smith",
                CreatedAt: DateTime.Now.AddDays(-7),
                UpdatedAt: DateTime.Now.AddDays(-2)
            ),
            new TodoDto(
                Id: 6,
                Title: "Prepare monthly report",
                Description: "Compile and prepare the monthly progress report",
                DueDate: DateTime.Now.AddDays(-1),
                IsCompleted: false,
                Tags: new[] { "Report", "Important" },
                AssignedTo: "Mike Johnson",
                CreatedAt: DateTime.Now.AddDays(-5),
                UpdatedAt: DateTime.Now.AddDays(-5)
            )
        };

        public TodoController([FromKeyedServices("TodoApi")] IApiClient apiClient)
        {
            _todoApiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        // GET: Todo/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: Todo/DashboardPartial
        public async Task<IActionResult> DashboardPartial()
        {
            var request = new HttpRequestBuilder()
                .WithMethod(HttpMethod.Get)
                .WithUri(new Uri("/todotasks", UriKind.Relative))
                .WithQueryParamsFromObject(new TodoTaskQueryParam
                {
                    Page = 1,
                    PageSize = 10,
                    SortBy = "duedate",
                    SortDirection = SortDirection.Asc
                });

            var response = await _todoApiClient.SendAsync<PagedResult<TaskDto>>(request);

            if (response?.Data == null)
            {
                ViewBag.Metrics = new Dictionary<string, object>();
                return PartialView("_DashboardPartial");
            }

            if (response?.Data == null)
            {
                // Handle API failure - use static data as fallback or return error
                ViewBag.Metrics = new Dictionary<string, object>();
                return PartialView("_DashboardPartial");
            }

            var tasks = response.Data;

            // Calculate metrics for the dashboard
            var metrics = new Dictionary<string, object>
            {                
                // Task completion metrics
                { "TotalTasks", _tasks.Count },
                { "CompletedTasks", _tasks.Count(t => t.IsCompleted) },
                { "PendingTasks", _tasks.Count(t => !t.IsCompleted) },
                { "CompletionRate", _tasks.Count > 0 ? Math.Round((double)_tasks.Count(t => t.IsCompleted) / _tasks.Count * 100, 1) : 0 },
                
                // Due date metrics
                { "OverdueTasks", _tasks.Count(t => !t.IsCompleted && t.DueDate < DateTime.Now.Date) },
                { "DueTodayTasks", _tasks.Count(t => !t.IsCompleted && t.DueDate.Date == DateTime.Now.Date) },
                { "DueThisWeekTasks", _tasks.Count(t => !t.IsCompleted && t.DueDate.Date > DateTime.Now.Date && t.DueDate.Date <= DateTime.Now.Date.AddDays(7)) },
                
                // Assignment metrics
                { "TasksByAssignee", _tasks.GroupBy(t => t.AssignedTo)
                                          .Select(g => new { Assignee = g.Key, Count = g.Count(), Completed = g.Count(t => t.IsCompleted) })
                                          .OrderByDescending(x => x.Count)
                                          .ToList() },
                
                // Tag metrics
                { "TasksByTag", _tasks.SelectMany(t => t.Tags.Select(tag => new { Tag = tag, IsCompleted = t.IsCompleted }))
                                     .GroupBy(t => t.Tag)
                                     .Select(g => new { Tag = g.Key, Count = g.Count(), Completed = g.Count(t => t.IsCompleted) })
                                     .OrderByDescending(x => x.Count)
                                     .ToList() },
                
                // Recent activity
                { "RecentTasks", _tasks.OrderByDescending(t => t.UpdatedAt).Take(5).ToList() }
            };

            ViewBag.Metrics = metrics;
            return PartialView("_DashboardPartial");
        }

        // GET: Todo
        public IActionResult Index()
        {
            return View();
        }

        // GET: Todo/TodoListPartial
        public async Task<IActionResult> TodoListPartialAsync(string filter = null)
        {
            var request = new HttpRequestBuilder()
              .WithMethod(HttpMethod.Get)
              .WithUri(new Uri("/todotasks", UriKind.Relative))
              .WithQueryParamsFromObject(new TodoTaskQueryParam
              {
                  Page = 1,
                  PageSize = 10,
                  SortBy = "duedate",
                  SortDirection = SortDirection.Asc
              });

            var response = await _todoApiClient.SendAsync<PagedResult<TaskDto>>(request);

            if (response?.Data == null)
            {
                ViewBag.Metrics = new Dictionary<string, object>();
                return PartialView("_DashboardPartial");
            }

            if (response?.Data == null)
            {
                // Handle API failure - use static data as fallback or return error
                ViewBag.Metrics = new Dictionary<string, object>();
                return PartialView("_DashboardPartial");
            }

            var _tasks = response.Data;

            IEnumerable<TaskDto> filteredTasks = _tasks;

            // Apply filters if specified
            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter.ToLower())
                {
                    case "completed":
                        filteredTasks = _tasks.Where(t => t.IsCompleted);
                        break;
                    case "pending":
                        filteredTasks = _tasks.Where(t => !t.IsCompleted);
                        break;
                    case "overdue":
                        filteredTasks = _tasks.Where(t => !t.IsCompleted && t.DueDate < DateTime.Now.Date);
                        break;
                }
            }

            ViewBag.Tasks = filteredTasks.ToList();
            ViewBag.Filter = filter;
            return PartialView("_TodoListPartial");
        }

        // GET: Todo/Detail
        public IActionResult Detail()
        {
            return View();
        }

        // GET: Todo/GetDetail/5
        public IActionResult GetDetail(int taskId)
        {
            var task = _tasks.Find(t => t.Id == taskId);

            if (task == null)
            {
                // If task not found in our dummy data, return a sample task
                task = new TodoDto(
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
            }

            return PartialView("_Detail", task);
        }

        // POST: Todo/Create
        [HttpPost]
        public IActionResult Create(string title, string description, DateTime dueDate, string assignedTo, string[] tags)
        {
            // Generate a new ID (in a real app, this would be handled by the database)
            int newId = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;

            // Create new task
            var newTask = new TodoDto(
                Id: newId,
                Title: title,
                Description: description,
                DueDate: dueDate,
                IsCompleted: false,
                Tags: tags ?? new string[0],
                AssignedTo: assignedTo,
                CreatedAt: DateTime.Now,
                UpdatedAt: DateTime.Now
            );

            // Add to our dummy data
            _tasks.Add(newTask);

            // Return JSON result for AJAX handling
            return Json(new { success = true, task = newTask });
        }

        // POST: Todo/Update/5
        [HttpPost]
        public IActionResult Update(int id, string title, string description, DateTime dueDate, bool isCompleted, string assignedTo, string[] tags)
        {
            // Find the task
            var taskIndex = _tasks.FindIndex(t => t.Id == id);

            if (taskIndex == -1)
            {
                return Json(new { success = false, message = "Task not found" });
            }

            // Update the task
            var updatedTask = new TodoDto(
                Id: id,
                Title: title,
                Description: description,
                DueDate: dueDate,
                IsCompleted: isCompleted,
                Tags: tags ?? new string[0],
                AssignedTo: assignedTo,
                CreatedAt: _tasks[taskIndex].CreatedAt,
                UpdatedAt: DateTime.Now
            );

            // Replace in our dummy data
            _tasks[taskIndex] = updatedTask;

            // Return JSON result for AJAX handling
            return Json(new { success = true, task = updatedTask });
        }

        // POST: Todo/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Find and remove the task
            var taskIndex = _tasks.FindIndex(t => t.Id == id);

            if (taskIndex == -1)
            {
                return Json(new { success = false, message = "Task not found" });
            }

            _tasks.RemoveAt(taskIndex);

            // Return JSON result for AJAX handling
            return Json(new { success = true });
        }

        // POST: Todo/ToggleComplete/5
        [HttpPost]
        public IActionResult ToggleComplete(int id)
        {
            // Find the task
            var taskIndex = _tasks.FindIndex(t => t.Id == id);

            if (taskIndex == -1)
            {
                return Json(new { success = false, message = "Task not found" });
            }

            // Toggle completion status
            var task = _tasks[taskIndex];
            var updatedTask = new TodoDto(
                Id: task.Id,
                Title: task.Title,
                Description: task.Description,
                DueDate: task.DueDate,
                IsCompleted: !task.IsCompleted,
                Tags: task.Tags,
                AssignedTo: task.AssignedTo,
                CreatedAt: task.CreatedAt,
                UpdatedAt: DateTime.Now
            );

            // Replace in our dummy data
            _tasks[taskIndex] = updatedTask;

            // Return JSON result for AJAX handling
            return Json(new { success = true, task = updatedTask });
        }
    }
}
