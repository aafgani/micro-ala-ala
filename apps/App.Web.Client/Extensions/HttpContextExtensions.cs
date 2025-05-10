using System.Security.Claims;

namespace App.Web.Client.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(i => i.Type == "name")?.Value ?? string.Empty;
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(i => i.Type == "preferred_username")?.Value ?? string.Empty;
        }

        public static string GetIdentifier(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        public static string GetSessionId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(i => i.Type == "session_id")?.Value ?? string.Empty;
        }
    }
}
