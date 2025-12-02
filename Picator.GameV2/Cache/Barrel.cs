using LiteDB;
using Mafiator.Common.Client.Cache;
using MemoryPack;
using Picator.Common.Client.Cache;

namespace Picator.Game.Cache;

public class Barrel : IBarrel
{
    public static string ApplicationId { get; set; } = string.Empty;
    public static string EncryptionKey { get; set; } = string.Empty;

    private static readonly Lazy<string> BaseCacheDir =
        new Lazy<string>(() => Path.Combine(BarrelUtils.GetBasePath(ApplicationId), "Cache"));

    public bool AutoExpire { get; set; }

    private static Barrel _instance = null;
    private static ILiteCollection<Banana> _col;

    /// <summary>
    /// Gets the instance of the Barrel
    /// </summary>
    public static IBarrel Current => _instance ??= new Barrel();

    public static IBarrel Create(string cacheDirectory, bool cache = false)
    {
        if (!cache)
            return new Barrel(cacheDirectory);

        return _instance ??= new Barrel(cacheDirectory);
    }

    private Barrel(string cacheDirectory = null)
    {
        var directory = string.IsNullOrEmpty(cacheDirectory) ? BaseCacheDir.Value : cacheDirectory;
        var path = Path.Combine(directory, "Barrel.db");
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (!string.IsNullOrWhiteSpace(EncryptionKey))
            path = $"Filename={path}; Password={EncryptionKey}";

        var db = new LiteDatabase(path);
        _col = db.GetCollection<Banana>();
    }

    #region Exist and Expiration Methods

    /// <summary>
    /// Checks to see if the key exists in the Barrel.
    /// </summary>
    /// <param name="key">Unique identifier for the entry to check</param>
    /// <returns>If the key exists</returns>
    public bool Exists(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        var ent = _col.FindById(key);

        return ent != null;
    }

    /// <summary>
    /// Checks to see if the entry for the key is expired.
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>If the expiration data has been met</returns>
    public bool IsExpired(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        var ent = _col.FindById(key);

        if (ent == null)
            return true;

        return DateTime.UtcNow > ent.ExpirationDate.ToUniversalTime();
    }

    #endregion

    #region Get Methods

    /// <summary>
    /// Gets all the keys that are saved in the cache
    /// </summary>
    /// <returns>The IEnumerable of keys</returns>
    public IEnumerable<string> GetKeys(CacheState state = CacheState.Active)
    {
        var allBananas = _col.FindAll();

        if (allBananas == null)
            return Array.Empty<string>();
        var bananas = new List<Banana>();

        if (state.HasFlag(CacheState.Active))
        {
            bananas = allBananas
                .Where(x => GetExpiration(x.Id) >= DateTime.UtcNow)
                .ToList();
        }

        if (state.HasFlag(CacheState.Expired))
            bananas.AddRange(allBananas.Where(x => GetExpiration(x.Id) < DateTime.UtcNow));

        return bananas.Select(x => x.Id);
    }

    /// <summary>
    /// Gets the data entry for the specified key.
    /// </summary>
    /// <param name="key">Unique identifier for the entry to get</param>
    /// <param name="options">Custom MessagePack serialization settings to use</param>
    /// <returns>The data object that was stored if found, else default(T)</returns>
    public T Get<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        var result = default(T);

        var ent = _col.FindById(key);

        if (ent == null || AutoExpire && IsExpired(key))
            return result;
        return MemoryPackSerializer.Deserialize<T>(ent.Contents);
    }


    /// <summary>
    /// Gets the ETag for the specified key.
    /// </summary>
    /// <param name="key">Unique identifier for entry to get</param>
    /// <returns>The ETag if the key is found, else null</returns>
    public string GetETag(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        var ent = _col.FindById(key);

        return ent?.ETag;
    }

    /// <summary>
    /// Gets the DateTime that the item will expire for the specified key.
    /// </summary>
    /// <param name="key">Unique identifier for entry to get</param>
    /// <returns>The expiration date if the key is found, else null</returns>
    public DateTime? GetExpiration(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        var ent = _col.FindById(key);

        return ent?.ExpirationDate;
    }

    #endregion

    #region Add Methods

    /// <summary>
    /// Adds a string netry to the barrel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Unique identifier for the entry</param>
    /// <param name="data">Data string to store</param>
    /// <param name="expireIn">Time from UtcNow to expire entry in</param>
    /// <param name="eTag">Optional eTag information</param>
    private static void Add(string key, byte[] data, TimeSpan expireIn, string eTag = null)
    {
        if (data == null)
            return;

        var ent = new Banana
        {
            Id = key,
            ExpirationDate = BarrelUtils.GetExpiration(expireIn),
            ETag = eTag,
            Contents = data
        };

        _col.Upsert(ent);
    }

    /// <summary>
    /// Adds an entry to the barrel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Unique identifier for the entry</param>
    /// <param name="data">Data object to store</param>
    /// <param name="expireIn">Time from UtcNow to expire entry in</param>
    /// <param name="eTag">Optional eTag information</param>
    public void Add<T>(string key, T data, TimeSpan expireIn, string eTag = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        if (data == null)
            throw new ArgumentNullException("Data can not be null.", nameof(data));

        var dataMsgPack = MemoryPackSerializer.Serialize(data);

        Add(key, dataMsgPack, expireIn, eTag);
    }

    #endregion

    #region Empty Methods

    /// <summary>
    /// Empties all expired entries that are in the Barrel.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    public void EmptyExpired() => _col.DeleteMany(b => b.ExpirationDate < DateTime.UtcNow);

    /// <summary>
    /// Empties all expired entries that are in the Barrel.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    public void EmptyAll() => _col.DeleteMany(b => b.Id != null);

    /// <summary>
    /// Empties all specified entries regardless if they are expired.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    /// <param name="key">keys to empty</param>
    public void Empty(params string[] key)
    {
        foreach (var k in key)
        {
            if (string.IsNullOrWhiteSpace(k))
                continue;

            _col.Delete(k);
        }
    }

    #endregion
}