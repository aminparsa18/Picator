namespace Picator.Common.Data.Dtos.Data.Dtos.Api;

/// <summary>
/// Api result status code.
/// </summary>
public enum ApiResultStatusCode
{
    /// <summary>
    /// Success.
    /// </summary>
    Success = 0,

    /// <summary>
    /// Server error.
    /// </summary>
    ServerError = 1,

    /// <summary>
    /// Bad request.
    /// </summary>
    BadRequest = 2,

    /// <summary>
    /// Not found.
    /// </summary>
    NotFound = 3,

    /// <summary>
    /// List empty.
    /// </summary>
    ListEmpty = 4,

    /// <summary>
    /// Logic error.
    /// </summary>
    LogicError = 5,

    /// <summary>
    /// Unauthorized.
    /// </summary>
    Unauthorized = 6,

    /// <summary>
    /// Forbidden.
    /// </summary>
    Forbidden = 7,

    /// <summary>
    /// Conflict.
    /// </summary>
    Conflict = 8,
}