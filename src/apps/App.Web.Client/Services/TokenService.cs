using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using App.Common.Domain.Configuration;
using System.Security.Claims;
using Microsoft.Identity.Client;

namespace App.Web.Client.Services;

public interface ITokenService
{
    Task<string?> GetAccessTokenAsync(ClaimsPrincipal user, string[] scopes, CancellationToken cancellationToken = default);
    Task<string?> GetCachedAccessTokenAsync(string userId);
}

public class TokenService : ITokenService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<TokenService> _logger;

    private readonly IConfiguration _config;

    public TokenService(IConfiguration config, IMemoryCache cache, ILogger<TokenService> logger)
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
    public async Task<string?> GetAccessTokenAsync(ClaimsPrincipal user, string[] scopes, CancellationToken cancellationToken = default)
    {
        // var oid = user.FindFirst("oid")?.Value;
        // var tid = user.FindFirst("tid")?.Value;
        var oid = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        var tid = user.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
        var accountId = $"{oid}.{tid}";

        var cacheKey = $"access_token_{accountId}_{string.Join("_", scopes)}";

        if (_cache.TryGetValue(cacheKey, out string accessToken))
        {
            return accessToken;
        }

        var cca = ConfidentialClientApplicationBuilder
            .Create(_config["AzureEntra:ClientId"])
            .WithClientSecret(_config["AzureEntra:ClientSecret"])
            .WithRedirectUri(_config["AzureEntra:RedirectUri"])
            .WithAuthority(new Uri(_config["AzureEntra:Authority"]))
            .Build();

        var account = await cca.GetAccountAsync(accountId);

        if (account == null)
        {
            throw new UnauthorizedAccessException("No MSAL account found in cache.");
        }

        try
        {
            var result = await cca.AcquireTokenSilent(scopes, account).ExecuteAsync();

            accessToken = result.AccessToken;

            // Cache token for its duration minus a buffer
            _cache.Set(cacheKey, accessToken, TimeSpan.FromMinutes(50));

            return accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to acquire access token for user {UserId}",
                user.GetObjectId() ?? user.GetNameIdentifierId());
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
