using System;

namespace App.Web.UI.Utilities.Http;

public class TokenService : ITokenService
{
    public Task<string?> GetAccessTokenAsync(string accountId, string[] scopes, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
