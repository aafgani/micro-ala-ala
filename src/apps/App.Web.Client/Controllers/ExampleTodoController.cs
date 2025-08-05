using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Web.Client.Services;
using System.Net.Http.Headers;

namespace App.Web.Client.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExampleTodoController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<TodoController> _logger;

    public ExampleTodoController(
        ITokenService tokenService,
        HttpClient httpClient,
        ILogger<TodoController> logger)
    {
        _tokenService = tokenService;
        _httpClient = httpClient;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTodos(CancellationToken cancellationToken)
    {
        try
        {
            // ✅ Acquire access token using the token service
            var accessToken = await _tokenService.GetAccessTokenAsync(User, cancellationToken);

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("Failed to acquire access token for user {UserId}", User.Identity?.Name);
                return Unauthorized("Unable to acquire access token");
            }

            // ✅ Add the access token to the HTTP client authorization header
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            // ✅ Call the downstream Todo API
            var response = await _httpClient.GetAsync("https://your-todo-api.azurewebsites.net/api/todos", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return Ok(content);
            }

            _logger.LogWarning("Todo API returned {StatusCode}: {ReasonPhrase}",
                response.StatusCode, response.ReasonPhrase);
            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching todos for user {UserId}", User.Identity?.Name);
            return StatusCode(500, "An error occurred while fetching todos");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] object todoData, CancellationToken cancellationToken)
    {
        try
        {
            // ✅ Acquire access token using the token service
            var accessToken = await _tokenService.GetAccessTokenAsync(User, cancellationToken);

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("Failed to acquire access token for user {UserId}", User.Identity?.Name);
                return Unauthorized("Unable to acquire access token");
            }

            // ✅ Add the access token to the HTTP client authorization header
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            // ✅ Create JSON content
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(todoData),
                System.Text.Encoding.UTF8,
                "application/json");

            // ✅ Call the downstream Todo API
            var response = await _httpClient.PostAsync(
                "https://your-todo-api.azurewebsites.net/api/todos",
                jsonContent,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return Ok(content);
            }

            _logger.LogWarning("Todo API returned {StatusCode}: {ReasonPhrase}",
                response.StatusCode, response.ReasonPhrase);
            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating todo for user {UserId}", User.Identity?.Name);
            return StatusCode(500, "An error occurred while creating todo");
        }
    }
}
