using System.Net;
using App.Common.Domain.Dtos.ApiResponse;
using FluentValidation;

namespace App.Api.Todo.Dtos.Validators;

public static class ValidationExtensions
{
    public static async Task<Result<T, ApiError>> ValidateAsync<T>(
            this T model,
            IValidator<T> validator)
    {
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return new ApiError(errors, HttpStatusCode.BadRequest);

        }
        return Result<T, ApiError>.Success(model);
    }
}
