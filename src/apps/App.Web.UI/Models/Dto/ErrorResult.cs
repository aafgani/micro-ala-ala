using System.Net;
using App.Common.Domain.Dtos.ApiResponse;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Models.Dto;

public sealed class ErrorResult<TError> : IActionResult
{
    private readonly TError _error;

    public ErrorResult(TError error)
    {
        _error = error;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        throw new NotImplementedException();
    }

    // public Task ExecuteResultAsync(ActionContext context)
    // {
    //     var statusCode = _error switch
    //     {
    //         ApiError apiError => apiError.StatusCode,
    //         string errorMessage when errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase) => HttpStatusCode.NotFound,
    //         string errorMessage when errorMessage.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) => HttpStatusCode.Unauthorized,
    //         _ => HttpStatusCode.BadRequest
    //     };

    //     context.Response.StatusCode = (int)statusCode;
    //     return context.Response.WriteAsJsonAsync(_error);
    // }
}