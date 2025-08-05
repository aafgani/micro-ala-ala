using Microsoft.Identity.Web;
using Microsoft.Identity.Client;
using App.Common.Domain.Auth;
using App.Common.Infrastructure.Cache;

namespace App.Web.Client.Utilities.Http;

public interface ITokenService
{
    Task<string?> GetAccessTokenAsync(string accountId, string[] scopes, CancellationToken cancellationToken = default);
}

public class TokenService : ITokenService
{
    private readonly ICacheService _cache;
    private readonly ILogger<TokenService> _logger;

    private readonly IConfiguration _config;

    public TokenService(IConfiguration config, ILogger<TokenService> logger, ICacheService cache)
    {
        _cache = cache;
        _logger = logger;
        _config = config;
    }

    /// <summary>
    /// Acquires an access token for the current user to call downstream APIs
    /// </summary>
    /// <param name="user">The current user's ClaimsPrincipal</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token string or null if acquisition fails</returns>
    public async Task<string?> GetAccessTokenAsync(string accountId, string[] scopes, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(accountId))
            {
                _logger.LogError("Unable to determine account ID for user");
                return null;
            }

            // 🔧 Check cache first
            var result = await _cache.FetchFromCacheOrDataSourceAsync<TokenResult>(
                $"todoapi-access-token-{accountId}",
                async (ct) =>
                {
                    // 🔧 Create MSAL app instance
                    var app = ConfidentialClientApplicationBuilder
                        .Create(_config["AzureEntra:ClientId"])
                        .WithClientSecret(_config["AzureEntra:ClientSecret"])
                        .WithRedirectUri(_config["AzureEntra:RedirectUri"])
                        .WithAuthority(new Uri(_config["AzureEntra:Authority"]))
                        .Build();

                    app.AddInMemoryTokenCache();

                    _logger.LogDebug("Using account ID: {AccountId} for token acquisition", accountId);

                    // 🔧 Get account from MSAL cache using stored account ID
                    var account = await app.GetAccountAsync(accountId);

                    if (account == null)
                    {
                        _logger.LogWarning("No MSAL account found in cache for account ID: {AccountId}", accountId);
                        return null;
                    }

                    // 🔧 Acquire token silently
                    var result = await app.AcquireTokenSilent(scopes, account).ExecuteAsync(cancellationToken);

                    _logger.LogDebug("Successfully acquired token for account {AccountId}", accountId);

                    var cacheExpiration = result.ExpiresOn.Subtract(DateTimeOffset.UtcNow).Subtract(TimeSpan.FromMinutes(5));
                    return new TokenResult(result.AccessToken, cacheExpiration);
                },
                CacheDefaults.DefaultTimeout, // default cache duration
                cancellationToken,
                result => result.CacheDuration); // will be overridden by the token's expiration

            return result.AccessToken;
        }
        catch (MsalUiRequiredException ex)
        {
            _logger.LogWarning("Interactive authentication required for user: {Error}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to acquire access token for scopes: {Scopes}", string.Join(", ", scopes));
            return null;
        }
    }
}
