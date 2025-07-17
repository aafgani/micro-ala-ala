using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using App.Common.Domain.Configuration;
using System.Security.Claims;

namespace App.Web.Client.Services;

public interface ITokenService
{
    Task<string?> GetAccessTokenAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
    Task<string?> GetCachedAccessTokenAsync(string userId);
}

public class TokenService : ITokenService
{
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IMemoryCache _memoryCache;
    private readonly TodoApi _todoApiConfig;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        ITokenAcquisition tokenAcquisition,
        IMemoryCache memoryCache,
        IOptions<TodoApi> todoApiConfig,
        ILogger<TokenService> logger)
    {
        _tokenAcquisition = tokenAcquisition;
        _memoryCache = memoryCache;
        _todoApiConfig = todoApiConfig.Value;
        _logger = logger;
    }

    /// <summary>
    /// Acquires an access token for the current user to call downstream APIs
    /// </summary>
    /// <param name="user">The current user's ClaimsPrincipal</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token string or null if acquisition fails</returns>
    public async Task<string?> GetAccessTokenAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        try
        {
            var scopes = _todoApiConfig.Scopes?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                ?? Array.Empty<string>();

            if (!scopes.Any())
            {
                _logger.LogWarning("No scopes configured for TodoApi");
                return null;
            }

            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(
                scopes,
                user: user);

            // Cache the token for quick retrieval
            var userId = user.GetObjectId() ?? user.GetNameIdentifierId();
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(accessToken))
            {
                var cacheKey = $"user_token_{userId}";
                _memoryCache.Set(cacheKey, accessToken, TimeSpan.FromMinutes(55)); // Cache for 55 minutes
            }

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
        var cachedToken = _memoryCache.Get<string>(cacheKey);

        return Task.FromResult(cachedToken);
    }
}
