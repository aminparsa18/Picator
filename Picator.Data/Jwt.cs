namespace Picator.Data;

/// <summary>
/// Jwt options stored in app settings.
/// </summary>
public sealed class Jwt
{
    /// <summary>
    /// Jwt secret key.
    /// </summary>
    public string Secret { get; set; }

    /// <summary>
    /// Jwt token life time.
    /// </summary>
    public TimeSpan TokenLifeTime { get; set; }
}