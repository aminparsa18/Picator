using MemoryPack;
using System.Security.Cryptography;
using System.Text;

namespace Picator.Repository.Cache;

public class Barrel : IBarrel
{
    readonly ReaderWriterLockSlim _indexLocker;
    readonly Lazy<string> _baseDirectory;

    private Barrel(string? cacheDirectory = null)
    {
        _baseDirectory = new Lazy<string>(() => string.IsNullOrEmpty(cacheDirectory)
            ? Path.Combine(BarrelUtils.GetBasePath(ApplicationId), "MonkeyCacheFS")
            : cacheDirectory);
        _indexLocker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);


        _index = new Dictionary<string, Tuple<string, DateTime>>();

        LoadIndex();
        WriteIndex();
    }

    public static string ApplicationId { get; set; } = string.Empty;

    public bool AutoExpire { get; set; }

    static Barrel? _instance = null;

    /// <summary>
    /// Gets the instance of the Barrel
    /// </summary>
    public static IBarrel Current => _instance ??= new Barrel();

    public static IBarrel Create(string cacheDirectory) =>
        new Barrel(cacheDirectory);

    /// <summary>
    /// Adds an entry to the barrel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Unique identifier for the entry</param>
    /// <param name="data">Data object to store</param>
    /// <param name="expireIn">Time from UtcNow to expire entry in</param>
    /// <param name="eTag">Optional eTag information</param>
    /// <param name="options">Custom MessagePack serialization settings to use</param>
    public void Add<T>(string key,
        T data,
        TimeSpan expireIn,
        string? eTag = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        if (data == null)
            throw new ArgumentNullException(nameof(data), "Data can not be null.");

        var dataMsgPack = MemoryPackSerializer.Serialize(data);

        Add(key, dataMsgPack, expireIn, eTag);
    }

    /// <summary>
    /// Adds an entry to the barrel
    /// </summary>
    /// <param name="key">Unique identifier for the entry</param>
    /// <param name="data">Data object to store</param>
    /// <param name="expireIn">Time from UtcNow to expire entry in</param>
    /// <param name="eTag">Optional eTag information</param>
    void Add(string key, byte[] data, TimeSpan expireIn, string? eTag = null)
    {
        _indexLocker.EnterWriteLock();

        try
        {
            var hash = Hash(key);
            var path = Path.Combine(_baseDirectory.Value, hash);

            if (!Directory.Exists(_baseDirectory.Value))
                Directory.CreateDirectory(_baseDirectory.Value);

            File.WriteAllBytes(path, data);

            _index[key] = new Tuple<string, DateTime>(eTag ?? string.Empty, BarrelUtils.GetExpiration(expireIn));

            WriteIndex();
        }
        finally
        {
            _indexLocker.ExitWriteLock();
        }
    }

    /// <summary>
    /// Empties all specified entries regardless if they are expired.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    /// <param name="key">keys to empty</param>
    public void Empty(params string[] key)
    {
        _indexLocker.EnterWriteLock();

        try
        {
            foreach (var k in key)
            {
                if (string.IsNullOrWhiteSpace(k))
                    continue;

                var file = Path.Combine(_baseDirectory.Value, Hash(k));
                if (File.Exists(file))
                    File.Delete(file);

                _index.Remove(k);
            }

            WriteIndex();
        }
        finally
        {
            _indexLocker.ExitWriteLock();
        }
    }

    /// <summary>
    /// Empties all expired entries that are in the Barrel.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    public void EmptyAll()
    {
        _indexLocker.EnterWriteLock();

        try
        {
            foreach (var file in _index.Select(item => Hash(item.Key))
                         .Select(hash => Path.Combine(_baseDirectory.Value, hash))
                         .Where(File.Exists))
            {
                File.Delete(file);
            }

            _index.Clear();

            WriteIndex();
        }
        finally
        {
            _indexLocker.ExitWriteLock();
        }
    }

    /// <summary>
    /// Empties all expired entries that are in the Barrel.
    /// Throws an exception if any deletions fail and rolls back changes.
    /// </summary>
    public void EmptyExpired()
    {
        _indexLocker.EnterWriteLock();

        try
        {
            var expired = _index.Where(k => k.Value.Item2 < DateTime.UtcNow);

            var toRem = new List<string>();

            foreach (var (key, _) in expired)
            {
                var hash = Hash(key);
                var file = Path.Combine(_baseDirectory.Value, hash);
                if (File.Exists(file))
                    File.Delete(file);
                toRem.Add(key);
            }

            foreach (var key in toRem)
                _index.Remove(key);

            WriteIndex();
        }
        finally
        {
            _indexLocker.ExitWriteLock();
        }
    }

    /// <summary>
    /// Checks to see if the key exists in the Barrel.
    /// </summary>
    /// <param name="key">Unique identifier for the entry to check</param>
    /// <returns>If the key exists</returns>
    public bool Exists(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        var exists = false;

        _indexLocker.EnterReadLock();

        try
        {
            exists = _index.ContainsKey(key);
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }

        return exists;
    }

    /// <summary>
    /// Gets all the keys that are saved in the cache
    /// </summary>
    /// <returns>The IEnumerable of keys</returns>
    public IEnumerable<string> GetKeys(CacheState state = CacheState.Active)
    {
        _indexLocker.EnterReadLock();

        try
        {
            if (_index == null)
                return Array.Empty<string>();
            var bananas = new List<KeyValuePair<string, Tuple<string, DateTime>>>();

            if (state.HasFlag(CacheState.Active))
            {
                bananas = _index
                    .Where(x => x.Value.Item2 >= DateTime.UtcNow)
                    .ToList();
            }

            if (state.HasFlag(CacheState.Expired))
                bananas.AddRange(_index.Where(x => x.Value.Item2 < DateTime.UtcNow));

            return bananas.Select(x => x.Key);
        }
        catch (Exception)
        {
            return Array.Empty<string>();
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }
    }

    /// <summary>
    /// Gets the data entry for the specified key.
    /// </summary>
    /// <param name="key">Unique identifier for the entry to get</param>
    /// <param name="options">Custom MessagePack serialization settings to use</param>
    /// <returns>The data object that was stored if found, else default(T)</returns>
    public T? Get<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        var result = default(T);

        _indexLocker.EnterReadLock();

        try
        {
            var hash = Hash(key);
            var path = Path.Combine(_baseDirectory.Value, hash);

            if (_index.ContainsKey(key) && File.Exists(path) && (!AutoExpire || AutoExpire && !IsExpired(key)))
            {
                var contents = File.ReadAllBytes(path);
                if (BarrelUtils.IsString(result))
                {
                    object final = contents;
                    return (T)final;
                }

                result = MemoryPackSerializer.Deserialize<T>(contents);
            }
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }

        return result;
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

        DateTime? date = null;

        _indexLocker.EnterReadLock();

        try
        {
            if (_index.TryGetValue(key, out Tuple<string, DateTime>? value))
                date = value?.Item2;
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }

        return date;
    }

    /// <summary>
    /// Gets the ETag for the specified key.
    /// </summary>
    /// <param name="key">Unique identifier for entry to get</param>
    /// <returns>The ETag if the key is found, else null</returns>
    public string? GetETag(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

        string? etag = null;

        _indexLocker.EnterReadLock();

        try
        {
            if (_index.TryGetValue(key, out Tuple<string, DateTime>? value))
                etag = value?.Item1;
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }

        return etag;
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

        var expired = true;

        _indexLocker.EnterReadLock();

        try
        {
            if (_index.ContainsKey(key))
                expired = _index[key].Item2 < DateTime.UtcNow;
        }
        finally
        {
            _indexLocker.ExitReadLock();
        }

        return expired;
    }

    readonly Dictionary<string, Tuple<string, DateTime>> _index;

    private const string indexFilename = "idx.dat";

    private string? _indexFile;

    private void WriteIndex()
    {
        if (string.IsNullOrEmpty(_indexFile))
            _indexFile = Path.Combine(_baseDirectory.Value, indexFilename);
        if (!Directory.Exists(_baseDirectory.Value))
            Directory.CreateDirectory(_baseDirectory.Value);

        using var f = File.Open(_indexFile, FileMode.Create);
        using var sw = new StreamWriter(f);
        foreach (var (key, (item1, item2)) in _index)
        {
            var dtEpoch = DateTimeToEpochSeconds(item2);
            sw.WriteLine($"{key}\t{item1}\t{dtEpoch}");
        }
    }

    private void LoadIndex()
    {
        if (string.IsNullOrEmpty(_indexFile))
            _indexFile = Path.Combine(_baseDirectory.Value, indexFilename);

        if (!File.Exists(_indexFile))
            return;

        _index.Clear();

        using var f = File.OpenRead(_indexFile);
        using var sw = new StreamReader(f);
        string? line;
        while ((line = sw.ReadLine()) != null)
        {
            var parts = line.Split('\t');
            if (parts.Length != 3)
                continue;
            var key = parts[0];
            var etag = parts[1];
            var dt = parts[2];

            if (!string.IsNullOrEmpty(key) && int.TryParse(dt, out var secondsSinceEpoch) &&
                !_index.ContainsKey(key))
                _index.Add(key,
                    new Tuple<string, DateTime>(etag, EpochSecondsToDateTime(secondsSinceEpoch)));
        }
    }

    private static string Hash(string input)
    {
        var data = MD5.HashData(Encoding.Default.GetBytes(input));
        return BitConverter.ToString(data);
    }

    private static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    private static int DateTimeToEpochSeconds(DateTime date)
    {
        var diff = date - Epoch;
        return (int)diff.TotalSeconds;
    }

    static DateTime EpochSecondsToDateTime(int seconds) => Epoch + TimeSpan.FromSeconds(seconds);
}