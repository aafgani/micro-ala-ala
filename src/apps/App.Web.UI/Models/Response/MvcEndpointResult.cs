using App.Common.Domain.Dtos.ApiResponse;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Models.Response;

public sealed class MvcEndpointResult<TValue, TError> : IActionResult
{
    private readonly ObjectResult _result;

    private MvcEndpointResult(ObjectResult result)
    {
        _result = result;
    }

    public static implicit operator MvcEndpointResult<TValue, TError>(Result<TValue, TError> result) =>
        result.IsSuccess
            ? new MvcEndpointResult<TValue, TError>(new OkObjectResult(result.Value))
            : new MvcEndpointResult<TValue, TError>(new ObjectResult(result.Error)
            {
                StatusCode = StatusCodes.Status400BadRequest
            });

    public Task ExecuteResultAsync(ActionContext context)
        => _result.ExecuteResultAsync(context);
}

