using App.Common.Domain.Dtos.Todo;
using App.Common.Domain.Pagination;

namespace App.Web.UI.Utilities.Http;

public class TodoApiClient : ITodoApiClient
{
    private readonly ILogger<TodoApiClient> _logger;
    private readonly HttpClient _httpClient;

    public TodoApiClient(HttpClient httpClient, ILogger<TodoApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TodolistDto> CreateTodoAsync(TodolistDto todolistDto)
    {
        try
        {
            _logger.LogInformation("Creating todo: {@TodolistDto}", todolistDto);

            var response = await _httpClient.PostAsJsonAsync("/todos", todolistDto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to create todo. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, error);

                throw new HttpRequestException(
                    $"Failed to create todo. Status: {response.StatusCode}, Error: {error}");
            }

            var createdTodo = await response.Content.ReadFromJsonAsync<TodolistDto>();

            if (createdTodo == null)
            {
                throw new InvalidOperationException("Created todo response was null");
            }

            _logger.LogInformation("Successfully created todo: {@CreatedTodo}", createdTodo);
            return createdTodo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo: {@TodolistDto}", todolistDto);
            throw;
        }
    }

    public async Task<PagedResult<TodolistDto>> GetTodosAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/todos?page=" + pageNumber + "&pageSize=" + pageSize, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TodolistDto>>(cancellationToken: cancellationToken);
        if (result == null)
        {
            throw new InvalidOperationException("Failed to deserialize the response to PagedResult<TodolistDto>. The response content was null or invalid.");
        }
        return result;
    }
}
