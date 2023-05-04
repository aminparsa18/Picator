namespace Picator.Common.Extensions;

/// <summary>
/// Extension class for Exceptions.
/// </summary>
public static class ExceptionExtension
{
    /// <summary>
    /// Retrieves message detail of an exception.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns>Exception message.</returns>
    public static string DetailedMessage(this Exception exception) =>
        exception.InnerException != null ? exception.InnerException.Message : exception.Message;
}