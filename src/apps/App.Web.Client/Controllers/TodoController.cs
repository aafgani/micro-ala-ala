using App.Common.Domain.Auth;
using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;
using App.Web.Client.Extensions;
using App.Web.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Client.Controllers
{
    [Authorize(Policy = Policy.Todos)]
    public class TodoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public TodoController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITokenService tokenService)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["TodoApiBaseUrl"] ?? "http://localhost:8081";
            this._tokenService = tokenService;
            _configuration = configuration;
        }

        // GET: Todo/Dashboard
        [Authorize(Policy = Policy.TodosDashboard)]
        public IActionResult Dashboard()
        {
            return View();
        }

        // // GET: Todo/DashboardPartial
        // public async Task<IActionResult> DashboardPartial()
        // {
        //     var client = _httpClientFactory.CreateClient();
        //     var tasks = await client.GetFromJsonAsync<List<TodoDto>>(_apiBaseUrl + "/todotasks");

        //     // Calculate metrics for the dashboard
        //     var metrics = new Dictionary<string, object>
        //     {
        //         { "TotalTasks", tasks.Count },
        //         { "CompletedTasks", tasks.Count(t => t.IsCompleted) },
        //         { "PendingTasks", tasks.Count(t => !t.IsCompleted) },
        //         { "CompletionRate", tasks.Count > 0 ? Math.Round((double)tasks.Count(t => t.IsCompleted) / tasks.Count * 100, 1) : 0 },
        //         { "OverdueTasks", tasks.Count(t => !t.IsCompleted && t.DueDate < DateTime.Now.Date) },
        //         { "DueTodayTasks", tasks.Count(t => !t.IsCompleted && t.DueDate.Date == DateTime.Now.Date) },
        //         { "DueThisWeekTasks", tasks.Count(t => !t.IsCompleted && t.DueDate.Date > DateTime.Now.Date && t.DueDate.Date <= DateTime.Now.Date.AddDays(7)) },
        //         { "TasksByAssignee", tasks.GroupBy(t => t.AssignedTo)
        //                                   .Select(g => new { Assignee = g.Key, Count = g.Count(), Completed = g.Count(t => t.IsCompleted) })
        //                                   .OrderByDescending(x => x.Count)
        //                                   .ToList() },
        //         { "TasksByTag", tasks.SelectMany(t => t.Tags.Select(tag => new { Tag = tag, IsCompleted = t.IsCompleted }))
        //                              .GroupBy(t => t.Tag)
        //                              .Select(g => new { Tag = g.Key, Count = g.Count(), Completed = g.Count(t => t.IsCompleted) })
        //                              .OrderByDescending(x => x.Count)
        //                              .ToList() },
        //         { "RecentTasks", tasks.OrderByDescending(t => t.UpdatedAt).Take(5).ToList() }
        //     };

        //     ViewBag.Metrics = metrics;
        //     return PartialView("_DashboardPartial");
        // }

        // GET: Todo
        public IActionResult Index()
        {
            return View();
        }

        // GET: Todo/TodoListPartial
        public async Task<IActionResult> TodoListPartial(string filter = null)
        {
            try
            {
                foreach (var claim in HttpContext.User.Claims)
                {
                    Console.WriteLine($"{claim.Type} = {claim.Value}");
                }

                var scopes = _configuration["TodoApi:Scopes"]?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                var accessToken = await _tokenService.GetAccessTokenAsync(HttpContext.User, scopes);
                var client = _httpClientFactory.CreateClient();
                var pagedResult = await client.GetFromJsonAsync<PagedResult<TodolistDto>>(_apiBaseUrl + "/todos?Page=1&PageSize=10");

                IEnumerable<TodolistDto> filteredTasks = pagedResult?.Data ?? Enumerable.Empty<TodolistDto>();

                // if (!string.IsNullOrEmpty(filter))
                // {
                //     switch (filter.ToLower())
                //     {
                //         case "completed":
                //             filteredTasks = filteredTasks.Where(t => t.IsCompleted);
                //             break;
                //         case "pending":
                //             filteredTasks = filteredTasks.Where(t => !t.IsCompleted);
                //             break;
                //         case "overdue":
                //             filteredTasks = filteredTasks.Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value.Date < DateTime.Now.Date);
                //             break;
                //     }
                // }

                ViewBag.Tasks = filteredTasks.ToList();
                ViewBag.Filter = filter;
                ViewBag.Pagination = pagedResult?.Pagination;
                return PartialView("_TodoListPartial");
            }
            catch (Exception e)
            {
                // Log the exception (not implemented here)
                ViewBag.ErrorMessage = "An error occurred while fetching the tasks. Please try again later";
                throw;
            }

        }

        // GET: Todo/Detail
        public IActionResult Detail()
        {
            return View();
        }

        // GET: Todo/GetDetail/5
        public async Task<IActionResult> GetDetail(int taskId)
        {
            var client = _httpClientFactory.CreateClient();
            var task = await client.GetFromJsonAsync<TodolistDto>(_apiBaseUrl + $"/todos/{taskId}");

            if (task == null)
            {
                task = new TodolistDto
                {
                    Id = taskId,
                    Title = "Task not found",
                    Description = "The requested task does not exist."
                };
            }

            return PartialView("_Detail", task);
        }

        // POST: Todo/Create
        [HttpPost]
        public async Task<IActionResult> Create(string title, string description, DateTime dueDate, string assignedTo, string[] tags)
        {
            var client = _httpClientFactory.CreateClient();
            var newTask = new TodolistDto
            {
                Title = title,
                Description = description,
                // DueDate = dueDate,
                // IsCompleted = false, // Default to not completed
                // Tags = tags ?? new string[0],
                UserId = User.GetIdentifier()
            };

            var response = await client.PostAsJsonAsync(_apiBaseUrl + "/todos", newTask);
            if (response.IsSuccessStatusCode)
            {
                var createdTask = await response.Content.ReadFromJsonAsync<TodolistDto>();
                return Json(new { success = true, task = createdTask });
            }
            return Json(new { success = false, message = "Failed to create task" });
        }

        // POST: Todo/Update/5
        [HttpPost]
        public async Task<IActionResult> Update(int id, string title, string description, DateTime dueDate, bool isCompleted, string assignedTo, string[] tags)
        {
            var client = _httpClientFactory.CreateClient();
            var updatedTask = new TodolistDto
            {
                Id = id,
                Title = title,
                Description = description,
                // DueDate = dueDate,
                // IsCompleted = isCompleted,
                // Tags = tags ?? new string[0],
                UserId = User.GetIdentifier()
            };

            var response = await client.PutAsJsonAsync(_apiBaseUrl + $"/todos/{id}", updatedTask);
            if (response.IsSuccessStatusCode)
            {
                var task = await response.Content.ReadFromJsonAsync<TodolistDto>();
                return Json(new { success = true, task });
            }
            return Json(new { success = false, message = "Task not found" });
        }

        // POST: Todo/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync(_apiBaseUrl + $"/todos/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Task not found" });
        }

        // POST: Todo/ToggleComplete/5
        [HttpPost]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var task = await client.GetFromJsonAsync<TodolistDto>(_apiBaseUrl + $"/todos/{id}");
            if (task == null)
            {
                return Json(new { success = false, message = "Task not found" });
            }

            var updatedTask = new TodolistDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description
                // DueDate: task.DueDate,
                // IsCompleted: !task.IsCompleted, // Toggle completion status
                // Tags: task.Tags,
            };

            var response = await client.PutAsJsonAsync(_apiBaseUrl + $"/todotasks/{id}", updatedTask);
            if (response.IsSuccessStatusCode)
            {
                var resultTask = await response.Content.ReadFromJsonAsync<TodolistDto>();
                return Json(new
                {
                    success = true,
                    task = resultTask
                });
            }
            return Json(new { success = false, message = "Task not found" });
        }
    }
}