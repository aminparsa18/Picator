using System.Collections.ObjectModel;

namespace Picator.Game.Extensions;

public static class CollectionExtensions
{
    public static Collection<T> Shuffle<T>(this Collection<T> collection)
    {
        var random = new Random();
        for (int i = collection.Count - 1; i > 0; i--)
        {
            var k = random.Next(i + 1);
            (collection[i], collection[k]) = (collection[k], collection[i]);
        }
        return collection;
    }
}