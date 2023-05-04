namespace Picator.Common.Extensions;

/// <summary>
/// Extension class for IEnumerables.
/// </summary>
public static class EnumerableExtensions
{
    private static readonly Random random = new();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence">List of T.</param>
    /// <returns>T</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static T SelectRandom<T>(this IEnumerable<T> sequence)
    {
        if (sequence == null)
            throw new ArgumentNullException();

        if (!sequence.Any())
            throw new ArgumentException("The sequence is empty.");

        //optimization for ICollection<T>
        return sequence.ElementAt(random.Next(sequence.Count()));
    }
}