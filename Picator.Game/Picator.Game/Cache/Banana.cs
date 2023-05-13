namespace Picator.Common.Client.Cache;

/// <summary>
/// Data object for Barrel
/// </summary>
class Banana
{
    /// <summary>
    /// Unique Identifier
    /// </summary>
#if SQLITE
    [PrimaryKey]
#elif LITEDB
    [BsonId]
#endif
    public string Id { get; set; }


    /// <summary>
    /// Additional ETag to set for Http Caching
    /// </summary>
    public string ETag { get; set; }

    /// <summary>
    /// Main Contents.
    /// </summary>
    public byte[] Contents { get; set; }

    /// <summary>
    /// Expiration data of the object, stored in UTC
    /// </summary>
    public DateTime ExpirationDate { get; set; }
}