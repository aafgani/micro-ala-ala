namespace App.Web.UI.Utilities.Http;

public interface ITokenService
{
    Task<string?> GetAccessTokenAsync(string accountId, string[] scopes, CancellationToken cancellationToken = default);
}
