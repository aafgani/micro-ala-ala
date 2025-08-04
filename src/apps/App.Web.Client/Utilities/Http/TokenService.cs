using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Web;
using Microsoft.Identity.Client;

namespace App.Web.Client.Utilities.Http;

public interface ITokenService
{
    Task<string?> GetAccessTokenAsync(string accountId, string[] scopes, CancellationToken cancellationToken = default);
    Task<string?> GetCachedAccessTokenAsync(string userId);
}

public class TokenService : ITokenService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<TokenService> _logger;

    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(IConfiguration config, IMemoryCache cache, ILogger<TokenService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _cache = cache;
        _logger = logger;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
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

            _logger.LogDebug("Using account ID: {AccountId} for token acquisition", accountId);

            // 🔧 Check cache first
            var cacheKey = $"access_token_{accountId}_{string.Join("_", scopes)}";
            if (_cache.TryGetValue(cacheKey, out string? cachedToken))
            {
                _logger.LogDebug("Retrieved cached token for account {AccountId}", accountId);
                return cachedToken;
            }

            // 🔧 Create MSAL app instance
            var app = ConfidentialClientApplicationBuilder
                .Create(_config["AzureEntra:ClientId"])
                .WithClientSecret(_config["AzureEntra:ClientSecret"])
                .WithRedirectUri(_config["AzureEntra:RedirectUri"])
                .WithAuthority(new Uri(_config["AzureEntra:Authority"]))
                .Build();

            app.AddInMemoryTokenCache();

            // 🔧 Get account from MSAL cache using stored account ID
            var account = await app.GetAccountAsync(accountId);

            if (account == null)
            {
                _logger.LogWarning("No MSAL account found in cache for account ID: {AccountId}", accountId);
                return null;
            }

            // 🔧 Acquire token silently
            var result = await app.AcquireTokenSilent(scopes, account).ExecuteAsync(cancellationToken);

            // 🔧 Cache the token with expiration buffer
            var cacheExpiration = result.ExpiresOn.Subtract(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, result.AccessToken, cacheExpiration);

            _logger.LogDebug("Successfully acquired token for account {AccountId}", accountId);
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


    /// <summary>
    /// Retrieves a cached access token for the specified user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Cached access token or null if not found</returns>
    public Task<string?> GetCachedAccessTokenAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return Task.FromResult<string?>(null);

        var cacheKey = $"user_token_{userId}";
        var cachedToken = _cache.Get<string>(cacheKey);

        return Task.FromResult(cachedToken);
    }
}
