using RepoDb.Interfaces;
using System.Collections;

namespace Picator.Repository.Cache;

public class FastCache : ICache
{
    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public void Add<T>(string key, T value, int expiration = 180, bool throwException = true)
    {
        Barrel.Current.Add(key, value, TimeSpan.FromMinutes(expiration));
    }

    public void Add<T>(CacheItem<T> item, bool throwException = true)
    {
        Barrel.Current.Add(item.Key, item.Value, DateTime.Now - item.Expiration);

    }

    public void Clear()
    {
        Barrel.Current.EmptyAll();
    }

    public bool Contains(string key)
    {
        return Barrel.Current.Exists(key);
    }

    public CacheItem<T>? Get<T>(string key, bool throwException = true)
    {
        var item = Barrel.Current.Get<T>(key);
        if (item != null)
            return new CacheItem<T>(key, item);
        return null;
    }

    public void Remove(string key, bool throwException = true)
    {
        Barrel.Current.Empty(key);
    }

    public Task AddAsync<T>(string key, T value, int expiration = 180, bool throwException = true, CancellationToken cancellationToken = default)
    {
        return AddAsync(new CacheItem<T>(key, value, expiration), throwException, cancellationToken);
    }

    public Task AddAsync<T>(CacheItem<T> item, bool throwException = true, CancellationToken cancellationToken = default)
    {
        Add(item, throwException);
        return Task.CompletedTask;
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        Clear();
        return Task.CompletedTask;
    }

    public Task<bool> ContainsAsync(string key, CancellationToken cancellationToken = default) =>
        Task.FromResult(Contains(key));


    public Task<CacheItem<T>?> GetAsync<T>(string key, bool throwException = true, CancellationToken cancellationToken = default) =>
        Task.FromResult(Get<T>(key, throwException));

    public Task RemoveAsync(string key, bool throwException = true, CancellationToken cancellationToken = default)
    {
        Remove(key, throwException);
        return Task.CompletedTask;
    }
}