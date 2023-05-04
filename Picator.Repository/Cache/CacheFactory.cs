using RepoDb.Interfaces;

namespace Picator.Repository.Cache;

public static class CacheFactory
{
    private static readonly object _syncLock;
    private static ICache? _cache;

    static CacheFactory()
    {
        _syncLock = new object();
    }

    public static ICache GetCache()
    {
        if (_cache != null)
            return _cache;
        lock (_syncLock)
        {
            _cache ??= new FastCache();
        }
        return _cache;
    }
}