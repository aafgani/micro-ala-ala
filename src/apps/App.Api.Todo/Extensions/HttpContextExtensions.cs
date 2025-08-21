using System.Security.Claims;

namespace App.Api.Todo.Extensions;

public static class HttpContextExtensions
{
    public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Email)?.Value ?? string.Empty;
    }

    public static string GetName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims.FirstOrDefault(i => i.Type == "name")?.Value ?? string.Empty;
    }

}
