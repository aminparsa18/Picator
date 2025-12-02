using MemoryPack;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Picator.Game.Extensions;

public class MemoryPackMediaTypeFormatter : MediaTypeFormatter
{
    private static readonly MediaTypeHeaderValue _contentTypeMediaTypeHeader = MediaTypeHeaderValue.Parse(MemoryPackHttpClientExtensions.ContentTypeString);

    public static readonly MemoryPackMediaTypeFormatter DefaultInstance = new();

    public static readonly MediaTypeFormatter[] DefaultMediaTypeFormatters = { DefaultInstance };

    private static MediaTypeFormatterCollection _defaultMediaTypeFormatterCollection;

    public static MediaTypeFormatterCollection DefaultMediaTypeFormatterCollection
    {
        get
        {
            if (_defaultMediaTypeFormatterCollection != null) return _defaultMediaTypeFormatterCollection;
            _defaultMediaTypeFormatterCollection = new MediaTypeFormatterCollection();
            _defaultMediaTypeFormatterCollection.AddRange(DefaultMediaTypeFormatters);
            return _defaultMediaTypeFormatterCollection;
        }
    }

    public MemoryPackMediaTypeFormatter()
    {
        SupportedMediaTypes.Add(_contentTypeMediaTypeHeader);
    }

    public override bool CanReadType(Type type) => true;

    public override bool CanWriteType(Type type) => CanReadType(type);

    public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
    {
        if (headers == null)
            throw new ArgumentNullException(nameof(headers));

        headers.ContentType = _contentTypeMediaTypeHeader;
    }

    public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
        TransportContext transportContext) => await MemoryPackSerializer.SerializeAsync(type, writeStream, value).ConfigureAwait(false);

    public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
        TransportContext transportContext, CancellationToken cancellationToken) =>
        await MemoryPackSerializer.SerializeAsync(type, writeStream, value, MemoryPackSerializerOptions.Utf8, cancellationToken).ConfigureAwait(false);

    public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger,
        CancellationToken cancellationToken) =>
        await MemoryPackSerializer.DeserializeAsync(type, readStream, MemoryPackSerializerOptions.Utf8, cancellationToken);

    public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger) =>
        await MemoryPackSerializer.DeserializeAsync(type, readStream);
}