namespace Picator.Game.Helpers;

/// <summary>
/// Helper class for random functions.
/// </summary>
public class RandomHelper
{
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#$&*";

    private static readonly Random _random = new();

    /// <summary>
    /// Generate a random string from specified characters.
    /// </summary>
    /// <param name="length">Length of generated random string.</param>
    /// <returns>Random string.</returns>
    public static string CreateRandomText(int length)
    {
        Span<char> span = stackalloc char[length];
        for (int i = 0; i < length; i++)
        {
            span[i] = chars[_random.Next(chars.Length)];
        }

        return new string(span);
    }
}