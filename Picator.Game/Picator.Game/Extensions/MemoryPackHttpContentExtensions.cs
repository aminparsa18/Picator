using System.Net.Http;

namespace Picator.Game.Extensions;

public static class MemoryPackHttpContentExtensions
{
    public static async Task<T> ReadAsMemoryPackAsync<T>(this HttpContent content) =>
        await content.ReadAsAsync<T>(MemoryPackMediaTypeFormatter.DefaultMediaTypeFormatters).ConfigureAwait(false);
}