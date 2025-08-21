namespace App.Web.UI.Utilities.Http.Token;

public interface ITokenService
{
    Task<string?> GetAccessTokenAsync(string accountId, string[] scopes, CancellationToken cancellationToken = default);
}
