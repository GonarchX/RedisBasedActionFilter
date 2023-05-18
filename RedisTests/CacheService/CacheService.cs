using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace RedisTests.CacheService;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<string?> GetCachedResponse(string key, CancellationToken cancellationToken)
    {
        return await _distributedCache.GetStringAsync(key, cancellationToken);
    }

    public async Task CacheResponse(
        string key,
        object value,
        double timeToLiveInSeconds,
        CancellationToken cancellationToken)
    {
        var serializedValue = JsonConvert.SerializeObject(value);
        await _distributedCache.SetStringAsync(
            key,
            serializedValue,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(timeToLiveInSeconds)
            },
            cancellationToken);
    }
}