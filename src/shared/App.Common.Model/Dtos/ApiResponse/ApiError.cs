using System.Net;

namespace App.Common.Domain.Dtos.ApiResponse;

public record ApiError(string Message, HttpStatusCode StatusCode = HttpStatusCode.BadRequest);