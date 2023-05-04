namespace Picator.Service.Contracts;

public interface IMemoryCache
{
    void SetCache<T>(T values, string key);
    T? GetCache<T>(string key) where T : class;
    void RemoveCache(string key);
}