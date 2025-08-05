using App.Common.Infrastructure.Cache;
using App.Web.Client.Models;
using App.Web.Client.Services.Abstractions;

namespace App.Web.Client.Services.Implementation
{
    public class UserSessionService : IUserSessionService
    {
        private readonly ICacheService _cache;

        public UserSessionService(ICacheService cache)
        {
            _cache = cache;
        }

        public async Task<bool> IsSessionExistsAsync(string userId, string sessionId)
        {
            return await _cache.IsExists<UserSession>($"session-{userId}");
        }

        public async Task SetUserSessionAsync(string userId, string sessionId, CancellationToken cancellationToken)
        {
            await _cache.FetchFromCacheOrDataSourceAsync(
                $"session-{userId}",
                async ct =>
                {
                    return new UserSession(userId, sessionId, DateTime.Now);
                },
                CacheDefaults.DefaultTimeout,
                cancellationToken);
        }
    }
}
