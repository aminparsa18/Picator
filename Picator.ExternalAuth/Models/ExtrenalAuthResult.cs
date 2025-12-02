namespace Picator.ExternalAuth;

public class ExtrenalAuthResult
{
    /// <summary>
    /// Flag indicating api call has been successfull.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Jwt token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Refresh token needed for refresh expired token.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Flag indicating api call has been successfull.
    /// </summary>
    public bool HasName { get; set; }
}