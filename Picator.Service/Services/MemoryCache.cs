using MemoryPack;
using Microsoft.Extensions.Caching.Distributed;
using Picator.Service.Contracts;

namespace Picator.Service.Services;

public class MemoryCache : IMemoryCache
{
    private readonly IDistributedCache _cache;

    public MemoryCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public void SetCache<T>(T values, string key)
    {
        var cacheOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpiration = DateTime.Now.AddHours(6),
            SlidingExpiration = TimeSpan.FromMinutes(3),
        };
        _cache.Set(key, MemoryPackSerializer.Serialize(values), cacheOptions);
    }

    public T? GetCache<T>(string key) where T : class
    {
        var values = _cache.Get(key);
        return values == null ? null : MemoryPackSerializer.Deserialize<T>(values);
    }

    public void RemoveCache(string key)
    {
        _cache.Remove(key);
    }
}