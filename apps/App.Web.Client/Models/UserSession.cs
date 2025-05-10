namespace App.Web.Client.Models
{
    public record UserSession(string UserId, string SessionId, DateTime IssuedAt);
}
