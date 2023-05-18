namespace RedisTests.CacheService;

public interface ICacheService
{
    Task<string?> GetCachedResponse(string key, CancellationToken cancellationToken);
    Task CacheResponse(string key, object value, double timeToLiveInSeconds, CancellationToken cancellationToken);
}