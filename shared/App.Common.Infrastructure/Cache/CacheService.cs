﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace App.Common.Infrastructure.Cache
{
    public class CacheService(IMemoryCache cache, ILogger<CacheService> logger) : ICacheService
    {
        private readonly IMemoryCache _cache = cache;
        private readonly ILogger<CacheService> _logger = logger;

        public async Task<T?> FetchFromCacheOrDataSourceAsync<T>(string cacheKey, Func<CancellationToken, Task<T?>> fallback, TimeSpan cacheTime, CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(cacheKey, out T? resultValue) ||
                resultValue == null)
            {
                resultValue = await fallback(cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetAbsoluteExpiration(cacheTime);

                _cache.Set(cacheKey, resultValue, cacheEntryOptions);
                _logger.LogTrace("Cached for {@cacheTime}. Cached key: {@cacheKey}", cacheTime, cacheKey);
            }
            else
            {
                _logger.LogTrace("Cache hit! Cached key: {@cacheKey}", cacheKey);
            }

            return resultValue;
        }

        public Task<bool> IsExists<T>(string cacheKey)
        {
            bool exists = _cache.TryGetValue(cacheKey, out T? resultValue) && resultValue != null;
            return Task.FromResult(exists);
        }

        public Task RemoveAsync(string sessionKey)
        {
            _cache.Remove(sessionKey);
            return Task.CompletedTask;
        }
    }
}
