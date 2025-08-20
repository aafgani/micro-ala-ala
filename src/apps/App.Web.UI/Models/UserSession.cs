namespace App.Web.UI.Models;

public record UserSession(string UserId, string SessionId, DateTime IssuedAt);

