namespace App.Web.Client.Services.Abstractions
{
    public interface IUserSessionService
    {
        Task SetUserSessionAsync(string userId, string sessionId, CancellationToken cancellationToken);
        Task<bool> IsSessionExistsAsync(string userId, string sessionId);
    }
}
