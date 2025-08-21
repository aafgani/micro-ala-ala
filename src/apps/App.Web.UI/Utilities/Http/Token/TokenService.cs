using System;
using App.Common.Domain.Auth;
using App.Common.Infrastructure.Cache;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace App.Web.UI.Utilities.Http.Token;

public class TokenService : ITokenService
{
    private readonly ICacheService _cache;
    private readonly ILogger<TokenService> _logger;
    private readonly IConfiguration _config;

    public TokenService(ICacheService cache, ILogger<TokenService> logger, IConfiguration config)
    {
        _cache = cache;
        _logger = logger;
        _config = config;
    }

    public async Task<string?> GetAccessTokenAsync(string accountId, string[] scopes, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(accountId))
            {
                _logger.LogError("Unable to determine account ID for user");
                return null;
            }

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
                               if (cacheExpiration < TimeSpan.FromMinutes(1))
                               {
                                   cacheExpiration = TimeSpan.FromMinutes(1); // Minimum cache duration
                               }
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
