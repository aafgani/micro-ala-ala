using System.Net;
using App.Common.Domain.Dtos.ApiResponse;

namespace App.Api.Todo.Models;

public sealed class ErrorResult<TError> : IResult
{
    private readonly TError _error;

    public ErrorResult(TError error)
    {
        _error = error;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        var statusCode = _error switch
        {
            ApiError apiError => apiError.StatusCode,
            string errorMessage when errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase) => HttpStatusCode.NotFound,
            string errorMessage when errorMessage.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.BadRequest
        };

        httpContext.Response.StatusCode = (int)statusCode;
        return httpContext.Response.WriteAsJsonAsync(_error);
    }
}