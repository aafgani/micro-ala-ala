namespace App.Common.Infrastructure.Cache
{
    public interface ICacheService
    {
        Task<bool> IsExists<T>(string cacheKey);

        Task<T?> FetchFromCacheOrDataSourceAsync<T>(
           string cacheKey,
           Func<CancellationToken, Task<T?>> fallback,
           TimeSpan cacheTime,
           CancellationToken cancellationToken,
           Func<T, TimeSpan>? getCustomExpiration = null);
        Task RemoveAsync(string sessionKey);
    }
}
