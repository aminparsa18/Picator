namespace Picator.Common.Extensions;

/// <summary>
/// Extension class for objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Check if object instance is null.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="name"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void CheckArgumentIsNull(this object obj, string name)
    {
        if (obj == null)
            throw new ArgumentNullException(name);
    }
}